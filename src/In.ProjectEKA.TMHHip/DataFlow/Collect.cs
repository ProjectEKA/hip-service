namespace In.ProjectEKA.TMHHip.DataFlow
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using HipLibrary.Patient;
    using HipLibrary.Patient.Model;
    using Hl7.Fhir.Model;
    using Hl7.Fhir.Serialization;
    using Model;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Optional;
    using Optional.Linq;
    using Serilog;
    using Code = Model.Code;
    using Coding = Model.Coding;
    using Endpoint = Model.Endpoint;
    using JsonSerializer = System.Text.Json.JsonSerializer;
    using Resource = Model.Resource;
    using System.IO;

    public class Collect : ICollect
    {
        private readonly HttpClient client;
        private readonly IPatientRepository patientRepository;

        public Collect(HttpClient client, IPatientRepository patientRepository)
        {
            this.client = client;
            this.client.Timeout = TimeSpan.FromSeconds(20);
            this.patientRepository = patientRepository;
            this.client.Timeout = TimeSpan.FromSeconds(20);
        }

        public async Task<Option<Entries>> CollectData(TraceableDataRequest dataRequest)
        {
            var parser = new FhirJsonParser();
            var careBundles = new List<CareBundle>();
            var fetchPatientDataTask = FetchPatientData(dataRequest);
            fetchPatientDataTask.Wait();
            var tmhPatientData = fetchPatientDataTask.Result;
            var caseId = dataRequest.CareContexts.First().PatientReference;
            var patient = patientRepository.PatientWith(caseId);
            var patientName = patient.Select(patientObj => patientObj.Name).ValueOr("");

            foreach (var hiType in dataRequest.HiType)
            {
                switch (hiType)
                {
                    case HiType.Observation:
                    {
                        careBundles.AddRange(ProcessObservationsData(dataRequest, tmhPatientData, patientName));
                        break;
                    }
                    case HiType.MedicationRequest:
                    {
                        if (tmhPatientData.Prescriptions != null && tmhPatientData.Prescriptions.Any())
                        {
                            var medicationResponse =
                                FindMedicationRequestData(dataRequest, tmhPatientData.Prescriptions, patientName)
                                    .GetAwaiter().GetResult();
                            if (medicationResponse.HasValue)
                            {
                                var serializeObject = JsonConvert.SerializeObject(
                                    medicationResponse.ValueOr((MedicationResponse) null),
                                    new JsonSerializerSettings
                                    {
                                        Formatting = Formatting.Indented,
                                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                                    });
                                var careBundle = new CareBundle(caseId, parser.Parse<Bundle>(serializeObject));
                                careBundles.Add(careBundle);
                            }
                        }

                        break;
                    }
                    case HiType.Condition:
                    {
                        if (tmhPatientData.SwellingSymptomsData != null && tmhPatientData.SwellingSymptomsData.Any())
                        {
                            var symptomResponse = FetchSwellingSymptomData(dataRequest,
                                tmhPatientData.SwellingSymptomsData, patientName).GetAwaiter().GetResult();
                            if (symptomResponse.HasValue)
                            {
                                var serializeObject = JsonConvert.SerializeObject(
                                    symptomResponse.ValueOr((ConditionResponse) null),
                                    new JsonSerializerSettings
                                    {
                                        Formatting = Formatting.Indented,
                                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                                    });
                                var careBundle = new CareBundle(caseId, parser.Parse<Bundle>(serializeObject));
                                careBundles.Add(careBundle);
                            }
                        }

                        break;
                    }
                    case HiType.DiagnosticReport:
                    {
                        if (tmhPatientData.DiagnosticReportAsPdfs != null &&
                            tmhPatientData.DiagnosticReportAsPdfs.Any())
                        {
                            var diagnosticReportResponse = FetchDiagnosticReportPdfsData(dataRequest,
                                tmhPatientData.DiagnosticReportAsPdfs, patientName).GetAwaiter().GetResult();
                            if (diagnosticReportResponse.HasValue)
                            {
                                var serializeObject = JsonConvert.SerializeObject(
                                    diagnosticReportResponse.ValueOr((DiagnosticReportResponse) null),
                                    new JsonSerializerSettings
                                    {
                                        Formatting = Formatting.Indented,
                                        NullValueHandling = NullValueHandling.Ignore,
                                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                                    });
                                var careBundle = new CareBundle(caseId, parser.Parse<Bundle>(serializeObject));
                                careBundles.Add(careBundle);
                            }
                        }

                        if (tmhPatientData.DiagnosticReportAsImages != null &&
                            tmhPatientData.DiagnosticReportAsImages.Any())
                        {
                            var diagnosticReportResponse = FetchDiagnosticReportImagesData(dataRequest,
                                tmhPatientData.DiagnosticReportAsImages, patientName).GetAwaiter().GetResult();
                            if (diagnosticReportResponse.HasValue)
                            {
                                var serializeObject = JsonConvert.SerializeObject(
                                    diagnosticReportResponse.ValueOr((DiagnosticReportResponse) null),
                                    new JsonSerializerSettings
                                    {
                                        Formatting = Formatting.Indented,
                                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                                    });
                                var careBundle = new CareBundle(caseId, parser.Parse<Bundle>(serializeObject));
                                careBundles.Add(careBundle);
                            }
                        }

                        careBundles.Add(new CareBundle(caseId, await ReadJsonAsync<Bundle>("MockedDicomImageData.json").ConfigureAwait(false)));
                        break;
                    }
                    case HiType.Medication:
                        break;
                    case HiType.DocumentReference:
                        break;
                    case HiType.Prescription:
                        break;
                    case HiType.DischargeSummary:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            var entries = new Entries(careBundles);
            return Option.Some(entries);
        }

        private static async Task<Option<DiagnosticReportResponse>> FetchDiagnosticReportImagesData(
            TraceableDataRequest dataRequest, List<DiagnosticReportAsImage> diagnosticReportAsImages,
            string patientName)
        {
            LogDataRequest(dataRequest);
            var uuid = Uuid.Generate().Value;
            var id = uuid.Split(":").Last();
            var representations = new List<IDiagnosticReport>();
            foreach (var reportAsImage in diagnosticReportAsImages)
            {
                if (!WithinRange(dataRequest.DateRange, reportAsImage.Issued))
                {
                    continue;
                }

                var imagingStudyUuid = Uuid.Generate().Value;
                var imagingStudyId = imagingStudyUuid.Split(":").Last();
                var diagRepImageRep = new DiagnosticReportImageRepresentation(uuid,
                    GetDiagRepResource(reportAsImage, imagingStudyId, id, patientName));
                representations.Add(diagRepImageRep);

                var endpointUuid = Uuid.Generate().Value;
                var endpointId = endpointUuid.Split(":").Last();
                var repImagingStudyText = new Text("generated",
                    "<div xmlns=\"http://www.w3.org/1999/xhtml\">Unstructured data can be sent here</div>");
                var diagRepImagingStudyRep =
                    new DiagnosticReportImagingStudyRepresentation(imagingStudyUuid, GetImagingStudyResource(endpointId,
                        reportAsImage,
                        repImagingStudyText, imagingStudyId));
                representations.Add(diagRepImagingStudyRep);

                var diagRepEndpointRep = new DiagnosticReportEndpointRepresentation(endpointUuid,
                    GetEndpointResource(reportAsImage, endpointId, repImagingStudyText));
                representations.Add(diagRepEndpointRep);
            }

            if (!representations.Any())
            {
                return Option.None<DiagnosticReportResponse>();
            }

            var diagnosticReportResponse = new DiagnosticReportResponse
            {
                Entry = representations,
                Id = Uuid.Generate().Value.Split(":").Last(),
                ResourceType = "Bundle",
                Type = "collection"
            };
            return Option.Some<DiagnosticReportResponse>(diagnosticReportResponse);
        }

        private static EndpointResource GetEndpointResource(DiagnosticReportAsImage reportAsImage,
            string endpointId, Text repImagingStudyText)
        {
            var connectionType =
                new ConnectionType("http://terminology.hl7.org/CodeSystem/endpoint-connection-type",
                    "dicom-wado-rs");
            return new EndpointResource(HiType.Endpoint, endpointId, repImagingStudyText, "active",
                connectionType, new List<PayloadType> {new PayloadType(reportAsImage.PayloadType)},
                new List<string> {reportAsImage.PayloadMimeType},
                reportAsImage.ReportUrl);
        }

        private static ImagingStudyResource GetImagingStudyResource(string endpointId,
            DiagnosticReportAsImage reportAsImage, Text repImagingStudyText, string imagingStudyId)
        {
            var subjectImagingStudy = new SubjectDiagReport(reportAsImage.SubjectReference);
            return new ImagingStudyResource(HiType.ImagingStudy, imagingStudyId,
                repImagingStudyText, subjectImagingStudy, "available", reportAsImage.StudyStartDate,
                reportAsImage.NumberOfSeries,
                reportAsImage.NumberOfInstances,
                new List<Endpoint> {new Endpoint("Endpoint" + "/" + endpointId)});
        }

        private static DiagnosticReportImage GetDiagRepResource(DiagnosticReportAsImage reportAsImage,
            string imagingStudyId, string id, string patientName)
        {
            var repText = new Text("additional",
                "<div xmlns=\"http://www.w3.org/1999/xhtml\">Unstructured data can be sent here</div>");
            var imagingStudy = new DiagnosticReportImagingStudy("ImagingStudy" + "/" + imagingStudyId);
            return new DiagnosticReportImage(HiType.DiagnosticReport, id, repText, "final",
                new Code(reportAsImage.ReportText), new Subject(patientName), reportAsImage.EffectiveDateTime,
                reportAsImage.Issued, new List<Performer> {new Performer(reportAsImage.Performer)},
                new List<DiagnosticReportImagingStudy> {imagingStudy},
                reportAsImage.ReportConclusion);
        }

        private static async Task<Option<DiagnosticReportResponse>> FetchDiagnosticReportPdfsData(
            TraceableDataRequest dataRequest, List<DiagnosticReportAsPdf> diagnosticReportAsPdfs, string patientName)
        {
            LogDataRequest(dataRequest);
            var representations = new List<IDiagnosticReport>();
            foreach (var reportAsPdf in diagnosticReportAsPdfs)
            {
                if (!WithinRange(dataRequest.DateRange, reportAsPdf.Issued))
                {
                    continue;
                }

                var uuid = Uuid.Generate().Value;
                var id = uuid.Split(":").Last();
                var pdfText =
                    new Text("additional",
                        "<div xmlns=\"http://www.w3.org/1999/xhtml\">Unstructured data can be sent here</div>");
                var code = new Code(reportAsPdf.ReportText);
                var subject = new Subject(patientName);
                var performer = new Performer(reportAsPdf.Performer);
                var pdfData = CustomWebclient.PdfToBase64(reportAsPdf.ReportUrl);
                var presentedForm = new PresentedForm(reportAsPdf.ContentType, pdfData, reportAsPdf.ReportTitle);
                var diagRepPdfResource = new DiagnosticReportPDFResource(HiType.DiagnosticReport, id, pdfText, "final",
                    code, subject, reportAsPdf.EffectiveDateTime, reportAsPdf.Issued, new List<Performer> {performer},
                    new List<PresentedForm> {presentedForm}, reportAsPdf.ReportConclusion);
                var diagRepRepresentation = new DiagnosticReportPdfRepresentation(uuid, diagRepPdfResource);
                representations.Add(diagRepRepresentation);
            }

            if (!representations.Any())
            {
                return Option.None<DiagnosticReportResponse>();
            }

            var diagnosticReportResponse = new DiagnosticReportResponse
            {
                Entry = representations,
                Id = Uuid.Generate().Value.Split(":").Last(),
                ResourceType = "Bundle",
                Type = "collection"
            };

            return Option.Some(diagnosticReportResponse);
        }

        private static IEnumerable<CareBundle> ProcessObservationsData(TraceableDataRequest dataRequest,
            PatientData tmhPatientData,
            string patientName)
        {
            var careBundles = new List<CareBundle>();
            var parser = new FhirJsonParser();
            var caseId = dataRequest.CareContexts.First().PatientReference;
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            if (tmhPatientData.ClinicalNotes != null && tmhPatientData.ClinicalNotes.Any())
            {
                var clinicalNotes = FetchClinicalNotes(dataRequest, tmhPatientData.ClinicalNotes, patientName)
                    .GetAwaiter().GetResult();
                if (clinicalNotes.HasValue)
                {
                    var serializeObject = JsonConvert.SerializeObject(clinicalNotes.ValueOr((ObservationResponse) null),
                        jsonSerializerSettings);
                    var careBundle = new CareBundle(caseId, parser.Parse<Bundle>(serializeObject));
                    careBundles.Add(careBundle);
                }
            }

            if (tmhPatientData.AllergiesData != null && tmhPatientData.AllergiesData.Any())
            {
                var allergiesData =
                    FetchAllergiesData(dataRequest, tmhPatientData.AllergiesData, patientName).GetAwaiter().GetResult();
                if (allergiesData.HasValue)
                {
                    var serializeObject = JsonConvert.SerializeObject(
                        allergiesData.ValueOr((ObservationResponse) null),
                        jsonSerializerSettings);
                    var careBundle = new CareBundle(caseId, parser.Parse<Bundle>(serializeObject));
                    careBundles.Add(careBundle);
                }
            }

            if (tmhPatientData.AbdomenExaminationsData != null && tmhPatientData.AbdomenExaminationsData.Any())
            {
                var abdomenExaminationsData = FetchAbdomenExaminationData(dataRequest,
                    tmhPatientData.AbdomenExaminationsData, patientName).GetAwaiter().GetResult();
                if (abdomenExaminationsData.HasValue)
                {
                    var serializeObject =
                        JsonConvert.SerializeObject(abdomenExaminationsData.ValueOr((ObservationResponse) null),
                            jsonSerializerSettings);
                    var careBundle = new CareBundle(caseId, parser.Parse<Bundle>(serializeObject));
                    careBundles.Add(careBundle);
                }
            }

            if (tmhPatientData.OralCavityExaminationsData != null && tmhPatientData.OralCavityExaminationsData.Any())
            {
                var oralCavityExaminationsData = FetchOralCavityExaminationsData(dataRequest,
                    tmhPatientData.OralCavityExaminationsData, patientName).GetAwaiter().GetResult();
                if (oralCavityExaminationsData.HasValue)
                {
                    var serializeObject = JsonConvert.SerializeObject(
                        oralCavityExaminationsData.ValueOr((ObservationResponse) null),
                        jsonSerializerSettings
                    );
                    var careBundle = new CareBundle(caseId, parser.Parse<Bundle>(serializeObject));
                    careBundles.Add(careBundle);
                }
            }

            if (tmhPatientData.SurgeryHistories != null && tmhPatientData.SurgeryHistories.Any())
            {
                var surgeryHistoriesData =
                    FetchSurgeryHistoryData(dataRequest, tmhPatientData.SurgeryHistories, patientName).GetAwaiter()
                        .GetResult();
                if (!surgeryHistoriesData.HasValue)
                {
                    return careBundles;
                }

                var serializeObject = JsonConvert.SerializeObject(
                    surgeryHistoriesData.ValueOr((ObservationResponse) null),
                    jsonSerializerSettings);
                var careBundle = new CareBundle(caseId, parser.Parse<Bundle>(serializeObject));
                careBundles.Add(careBundle);
            }

            return careBundles;
        }

        private static bool WithinRange(DateRange range, DateTime date)
        {
            var fromDate = ParseDate(range.From);
            var toDate = ParseDate(range.To);
            return date >= fromDate && date < toDate;
        }

        private static DateTime ParseDate(string dateString)
        {
            var formatStrings = new[]
            {
                "yyyy-MM-dd", "yyyy-MM-dd hh:mm:ss", "yyyy-MM-dd hh:mm:ss tt", "yyyy-MM-ddTHH:mm:ss.fff",
                "yyyy-MM-ddTHH:mm:ss.ffff", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm:ss.fffff",
                "yyyy-MM-ddTHH:mm:ss.ffffff", "dd/MM/yyyy", "dd/MM/yyyy hh:mm:ss", "dd/MM/yyyy hh:mm:ss tt",
                "dd/MM/yyyyTHH:mm:ss.fffzzz"
            };
            var tryParseExact = DateTime.TryParseExact(dateString,
                formatStrings,
                CultureInfo.CurrentCulture,
                DateTimeStyles.None,
                out var aDateTime);
            if (!tryParseExact)
            {
                Log.Error($"Error parsing date: {dateString}");
            }

            return aDateTime;
        }


        private static async Task<Option<MedicationResponse>> FindMedicationRequestData(
            TraceableDataRequest dataRequest,
            List<Prescription> prescriptions, string patientName)
        {
            LogDataRequest(dataRequest);
            var list = new List<IMedication>();

            foreach (var prescription in prescriptions)
            {
                if (!WithinRange(dataRequest.DateRange, prescription.Date))
                {
                    continue;
                }

                var uuid = Uuid.Generate().Value;
                var id = uuid.Split(":").Last();
                var uuidMedication = Uuid.Generate().Value;
                var medicationId = uuid.Split(":").Last();
                var medicationRequestRepresentation = new MedicationRequestRepresentation
                {
                    FullUrl = uuid,
                    Resource = new MedicationRequestResource
                    {
                        Id = id,
                        Intent = "order",
                        Status = "active",
                        Subject = new Subject(patientName),
                        AuthoredOn = prescription.Date.Date,
                        ResourceType = HiType.MedicationRequest,
                        DosageInstruction = new DosageInstruction(1, prescription.Dosage),
                        MedicationReference = new MedicationReference("Medication/" + medicationId, "Medications")
                    }
                };
                var coding =
                    new Coding(prescription.GenName, prescription.Medicine);
                var medicationRepresentation = new MedicationRepresentation
                {
                    FullUrl = uuidMedication,
                    Resource = new MedicationResource
                    {
                        Id = medicationId, ResourceType = HiType.Medication,
                        Code = new Code(new List<Coding> {coding}, prescription.Medicine)
                    }
                };

                list.Add(medicationRequestRepresentation);
                list.Add(medicationRepresentation);
            }

            if (!list.Any())
            {
                return Option.None<MedicationResponse>();
            }

            var medicationResponse = new MedicationResponse
            {
                Entry = list,
                Id = Uuid.Generate().Value.Split(":").Last(),
                ResourceType = "Bundle",
                Type = "collection"
            };
            return Option.Some(medicationResponse);
        }

        private static async Task<Option<ObservationResponse>> FetchClinicalNotes(TraceableDataRequest dataRequest,
            List<ClinicalNote> clinicalNotes, string patientName)
        {
            LogDataRequest(dataRequest);
            var uuid = Uuid.Generate().Value;
            var id = uuid.Split(":").Last();
            var observations = (from clinicalNote in clinicalNotes
                where WithinRange(dataRequest.DateRange, clinicalNote.CreatedDate)
                select new ObservationRepresentation
                {
                    FullUrl = uuid,
                    Resource = new Resource(HiType.Observation, id, "final", new Code("Clinical notes"),
                        new Subject(patientName), new List<Performer> {new Performer(clinicalNote.UserName)},
                        clinicalNote.CreatedDate, clinicalNote.Note),
                }).ToList();

            if (!observations.Any())
            {
                return Option.None<ObservationResponse>();
            }

            var observationResponse = new ObservationResponse
            {
                Entry = observations,
                Id = Uuid.Generate().Value.Split(":").Last(),
                ResourceType = "Bundle",
                Type = "collection"
            };
            return Option.Some(observationResponse);
        }

        private static async Task<Option<ConditionResponse>> FetchSwellingSymptomData(TraceableDataRequest dataRequest,
            List<SwellingSymptomData> swellingSymptomsData, string patientName)
        {
            LogDataRequest(dataRequest);
            var uuid = Uuid.Generate().Value;
            var id = uuid.Split(":").Last();
            var conditions = (from swellingSymptomData in swellingSymptomsData
                where WithinRange(dataRequest.DateRange, swellingSymptomData.RecordedDate)
                select new ConditionRepresentation
                {
                    FullUrl = uuid,
                    Resource = new ConditionResource(HiType.Condition, id, new Code("Symptoms of Swelling"),
                        new Subject(patientName), swellingSymptomData.RecordedDate,
                        new List<Note>
                        {
                            new Note(swellingSymptomData.SwellingSite), new Note(swellingSymptomData.SwellingLtrl),
                            new Note(swellingSymptomData.SwellingSize)
                        }),
                }).Cast<ICondition>().ToList();

            if (!conditions.Any())
            {
                return Option.None<ConditionResponse>();
            }

            var conditionResponse = new ConditionResponse
            {
                Entry = conditions,
                Id = Uuid.Generate().Value.Split(":").Last(),
                ResourceType = "Bundle",
                Type = "collection"
            };
            return Option.Some(conditionResponse);
        }

        private static async Task<Option<ObservationResponse>> FetchAllergiesData(TraceableDataRequest dataRequest,
            List<AllergyData> allergiesData, string patientName)
        {
            LogDataRequest(dataRequest);
            var uuid = Uuid.Generate().Value;
            var id = uuid.Split(":").Last();
            var observations = (from allergyData in allergiesData
                where WithinRange(dataRequest.DateRange, allergyData.AllergyDate)
                select new ObservationRepresentation
                {
                    FullUrl = uuid,
                    Resource = new Resource(HiType.Observation, id, "final", new Code("Allergies"),
                        new Subject(patientName), allergyData.AllergyDate, allergyData.Allergies,
                        new List<Note> {new Note(allergyData.AllergyRemark)}),
                }).ToList();

            if (!observations.Any())
            {
                return Option.None<ObservationResponse>();
            }

            var observationResponse = new ObservationResponse
            {
                Entry = observations,
                Id = Uuid.Generate().Value.Split(":").Last(),
                ResourceType = "Bundle",
                Type = "collection"
            };
            return Option.Some(observationResponse);
        }

        private static async Task<Option<ObservationResponse>> FetchAbdomenExaminationData(
            TraceableDataRequest dataRequest,
            List<AbdomenExaminationData> abdomenExaminationsData, string patientName)
        {
            LogDataRequest(dataRequest);
            var uuid = Uuid.Generate().Value;
            var id = uuid.Split(":").Last();
            var observations = (from abdomenExaminationData in abdomenExaminationsData
                where WithinRange(dataRequest.DateRange, abdomenExaminationData.EffectiveDateTime)
                select new ObservationRepresentation
                {
                    FullUrl = uuid,
                    Resource = new Resource(HiType.Observation, id, "final", new Code("Examination notes for Abdomen"),
                        new Subject(patientName), abdomenExaminationData.EffectiveDateTime,
                        abdomenExaminationData.CAbdomen),
                }).ToList();

            if (!observations.Any())
            {
                return Option.None<ObservationResponse>();
            }

            var observationResponse = new ObservationResponse
            {
                Entry = observations,
                Id = Uuid.Generate().Value.Split(":").Last(),
                ResourceType = "Bundle",
                Type = "collection"
            };
            return Option.Some(observationResponse);
        }

        private static async Task<Option<ObservationResponse>> FetchSurgeryHistoryData(TraceableDataRequest dataRequest,
            List<SurgeryHistory> surgeryHistories, string patientName)
        {
            LogDataRequest(dataRequest);
            var uuid = Uuid.Generate().Value;
            var id = uuid.Split(":").Last();
            var observations = (from surgeryHistory in surgeryHistories
                where WithinRange(dataRequest.DateRange, surgeryHistory.SurgeryWhen)
                select new ObservationRepresentation
                {
                    FullUrl = uuid,
                    Resource = new Resource(HiType.Observation, id, "final", new Code("Past history of surgery"),
                        new Subject(patientName), new List<Performer> {new Performer(surgeryHistory.HospitalDtls)},
                        surgeryHistory.SurgeryWhen, surgeryHistory.SurgeryDtls,
                        new List<Note> {new Note(surgeryHistory.SurgeryRemarks)}),
                }).ToList();

            if (!observations.Any())
            {
                return Option.None<ObservationResponse>();
            }

            var observationResponse = new ObservationResponse
            {
                Entry = observations,
                Id = Uuid.Generate().Value.Split(":").Last(),
                ResourceType = "Bundle",
                Type = "collection"
            };
            return Option.Some(observationResponse);
        }

        private static async Task<Option<ObservationResponse>> FetchOralCavityExaminationsData(
            TraceableDataRequest dataRequest,
            List<OralCavityExaminationData> oralCavityExaminationsData, string patientName)
        {
            LogDataRequest(dataRequest);
            var uuid = Uuid.Generate().Value;
            var id = uuid.Split(":").Last();
            var observations = new List<IObservation>();

            foreach (var oralCavityExaminationData in oralCavityExaminationsData)
            {
                if (!WithinRange(dataRequest.DateRange, oralCavityExaminationData.EffectiveDateTime))
                {
                    continue;
                }

                var tongueObservationUuid = Uuid.Generate().Value;
                var tongueObservationId = tongueObservationUuid.Split(":").Last();
                var buccalmcsaObservationUuid = Uuid.Generate().Value;
                var buccalmcsaObservationId = buccalmcsaObservationUuid.Split(":").Last();
                var hasMember = new List<Member>
                {
                    new Member("Observation/" + tongueObservationId, "TONGUE"),
                    new Member("Observation/" + buccalmcsaObservationId, "BUCCALMCSA")
                };
                var observationRepresentation = new ObservationRepresentation
                {
                    FullUrl = uuid,
                    Resource = new Resource(HiType.Observation, id, "final",
                        new Code("Examination notes for Oral cavity"), new Subject(patientName),
                        oralCavityExaminationData.EffectiveDateTime, hasMember),
                };
                var tongueResource = new Resource(HiType.Observation, tongueObservationId, "final",
                    new Code("TONGUE"), new Subject(patientName), oralCavityExaminationData.EffectiveDateTime,
                    oralCavityExaminationData.Tongue);
                var buccalmcsaResource = new Resource(HiType.Observation, buccalmcsaObservationId, "final",
                    new Code("BUCCALMCSA"), new Subject(patientName), oralCavityExaminationData.EffectiveDateTime,
                    oralCavityExaminationData.Buccalmcsa);
                observations.Add(observationRepresentation);
                observations.Add(new OralCavityExaminationObservationRepresention(tongueResource));
                observations.Add(new OralCavityExaminationObservationRepresention(buccalmcsaResource));
            }

            if (!observations.Any())
            {
                return Option.None<ObservationResponse>();
            }

            var oralCavityExmResponse = new ObservationResponse
            {
                Entry = observations,
                Id = Uuid.Generate().Value.Split(":").Last(),
                ResourceType = "Bundle",
                Type = "collection"
            };
            return Option.Some(oralCavityExmResponse);
        }

        private async Task<PatientData> FetchPatientData(
            TraceableDataRequest dataRequest)
        {
            try
            {
                LogDataRequest(dataRequest);
                var caseId = dataRequest.CareContexts.First().PatientReference;
                var request = new HttpRequestMessage(HttpMethod.Post, "https://tmc.gov.in/tmh_ncg_api/healthInfo")
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(new
                        {
                            HiTypes = dataRequest.HiType,
                            StartDate = dataRequest.DateRange.From,
                            EndDate = dataRequest.DateRange.To,
                            CaseId = caseId
                        }),
                        Encoding.UTF8,
                        "application/json")
                };

                var response = await client.SendAsync(request).ConfigureAwait(false);
                var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<PatientData>(responseStream);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        private static void LogDataRequest(TraceableDataRequest request)
        {
            var ccList = JsonConvert.SerializeObject(request.CareContexts);
            var requestedHiTypes = string.Join(", ", request.HiType.Select(hiType => hiType.ToString()));
            Log.Information("Data request received." +
                            $" transactionId:{request.TransactionId} , " +
                            $"CareContexts:{ccList}, " +
                            $"HiTypes:{requestedHiTypes}," +
                            $" From date:{request.DateRange.From}," +
                            $" To date:{request.DateRange.To}, " +
                            $"CallbackUrl:{request.DataPushUrl}, " +
                            $"CorrelationId:{request.CorrelationId}");
        }
        private static async Task<T> ReadJsonAsync<T>(string filePath) where T : Base
        {
            var jsonData = await File.ReadAllTextAsync(filePath);
            var fjp = new FhirJsonParser();
            return fjp.Parse<T>(jsonData);
        }
    }
}