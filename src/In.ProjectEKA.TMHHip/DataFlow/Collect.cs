using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using In.ProjectEKA.HipLibrary.Patient;
using In.ProjectEKA.HipLibrary.Patient.Model;
using In.ProjectEKA.TMHHip.DataFlow.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Optional;
using Optional.Linq;
using Serilog;
using Code = In.ProjectEKA.TMHHip.DataFlow.Model.Code;
using Coding = In.ProjectEKA.TMHHip.DataFlow.Model.Coding;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Resource = In.ProjectEKA.TMHHip.DataFlow.Model.Resource;

namespace In.ProjectEKA.TMHHip.DataFlow
{
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

        public async Task<Option<Entries>> CollectData(DataRequest dataRequest)
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
                        if (tmhPatientData.Prescriptions != null)
                        {
                            var medicationResponse =
                                FindMedicationRequestData(dataRequest, tmhPatientData.Prescriptions, patientName)
                                    .Result;
                            var serializeObject = JsonConvert.SerializeObject(medicationResponse,
                                new JsonSerializerSettings
                                {
                                    Formatting = Formatting.Indented,
                                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                                });
                            var careBundle = new CareBundle(caseId, parser.Parse<Bundle>(serializeObject));
                            careBundles.Add(careBundle);
                        }

                        break;
                    }
                    case HiType.Condition:
                    {
                        if (tmhPatientData.SwellingSymptomsData != null)
                        {
                            var symptomResponse = FetchSwellingSymptomData(dataRequest,
                                tmhPatientData.SwellingSymptomsData, patientName).Result;
                            var serializeObject = JsonConvert.SerializeObject(symptomResponse,
                                new JsonSerializerSettings
                                {
                                    Formatting = Formatting.Indented,
                                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                                });
                            var careBundle = new CareBundle(caseId, parser.Parse<Bundle>(serializeObject));
                            careBundles.Add(careBundle);
                        }

                        break;
                    }
                    case HiType.DiagnosticReport:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            var entries = new Entries(careBundles);
            return Option.Some(entries);
        }

        private IEnumerable<CareBundle> ProcessObservationsData(DataRequest dataRequest, PatientData tmhPatientData,
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

            if (tmhPatientData.ClinicalNotes != null)
            {
                var serializeObject = JsonConvert.SerializeObject(
                    FetchClinicalNotes(dataRequest, tmhPatientData.ClinicalNotes, patientName).Result,
                    jsonSerializerSettings);
                var careBundle = new CareBundle(caseId, parser.Parse<Bundle>(serializeObject));
                careBundles.Add(careBundle);
            }

            if (tmhPatientData.AllergiesData != null)
            {
                var serializeObject = JsonConvert.SerializeObject(
                    FetchAllergiesData(dataRequest, tmhPatientData.AllergiesData, patientName).Result,
                    jsonSerializerSettings);
                var careBundle = new CareBundle(caseId, parser.Parse<Bundle>(serializeObject));
                careBundles.Add(careBundle);
            }

            if (tmhPatientData.AbdomenExaminationsData != null)
            {
                var serializeObject = JsonConvert.SerializeObject(FetchAbdomenExaminationData(dataRequest,
                    tmhPatientData.AbdomenExaminationsData, patientName).Result, jsonSerializerSettings);
                var careBundle = new CareBundle(caseId, parser.Parse<Bundle>(serializeObject));
                careBundles.Add(careBundle);
            }

            if (tmhPatientData.OralCavityExaminationsData != null)
            {
                var serializeObject = JsonConvert.SerializeObject(FetchOralCavityExaminationsData(dataRequest,
                        tmhPatientData.OralCavityExaminationsData, patientName).Result, jsonSerializerSettings
                );
                var careBundle = new CareBundle(caseId, parser.Parse<Bundle>(serializeObject));
                careBundles.Add(careBundle);
            }

            if (tmhPatientData.SurgeryHistories != null)
            {
                var serializeObject = JsonConvert.SerializeObject(
                    FetchSurgeryHistoryData(dataRequest, tmhPatientData.SurgeryHistories, patientName).Result,
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


        private async Task<MedicationResponse> FindMedicationRequestData(DataRequest dataRequest,
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
                    new In.ProjectEKA.TMHHip.DataFlow.Model.Coding(prescription.GenName, prescription.Medicine);
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

            var medicationResponse = new MedicationResponse
            {
                Entry = list,
                Id = Uuid.Generate().Value.Split(":").Last(),
                ResourceType = "Bundle",
                Type = "collection"
            };
            return medicationResponse;
        }

        private async Task<ObservationResponse> FetchClinicalNotes(DataRequest dataRequest,
            List<ClinicalNote> clinicalNotes, string patientName)
        {
            LogDataRequest(dataRequest);
            var uuid = Uuid.Generate().Value;
            var id = uuid.Split(":").Last();
            var observations = new List<ObservationRepresentation>();

            foreach (var clinicalNote in clinicalNotes)
            {
                if (!WithinRange(dataRequest.DateRange, clinicalNote.CreatedDate))
                {
                    continue;
                }

                var observationRepresentation = new ObservationRepresentation
                {
                    FullUrl = uuid,
                    Resource = new Resource(HiType.Observation, id, "final",
                        new Code("Clinical notes"),
                        new Subject(patientName), new List<Performer> {new Performer(clinicalNote.UserName)},
                        clinicalNote.CreatedDate,
                        clinicalNote.Note),
                };
                observations.Add(observationRepresentation);
            }

            var observationResponse = new ObservationResponse
            {
                Entry = observations,
                Id = Uuid.Generate().Value.Split(":").Last(),
                ResourceType = "Bundle",
                Type = "collection"
            };
            return observationResponse;
        }

        private async Task<ConditionResponse> FetchSwellingSymptomData(DataRequest dataRequest,
            List<SwellingSymptomData> swellingSymptomsData, string patientName)
        {
            LogDataRequest(dataRequest);
            var uuid = Uuid.Generate().Value;
            var id = uuid.Split(":").Last();
            var conditions = new List<ICondition>();

            foreach (var swellingSymptomData in swellingSymptomsData)
            {
                if (!WithinRange(dataRequest.DateRange, swellingSymptomData.RecordedDate))
                {
                    continue;
                }

                var conditionRepresentation = new ConditionRepresentation
                {
                    FullUrl = uuid,
                    Resource = new ConditionResource(HiType.Condition, id,
                        new Code("Symptoms of Swelling"),
                        new Subject(patientName),
                        swellingSymptomData.RecordedDate,
                        new List<Note>
                        {
                            new Note(swellingSymptomData.SwellingSite), new Note(swellingSymptomData.SwellingLtrl),
                            new Note(swellingSymptomData.SwellingSize)
                        }),
                };
                conditions.Add(conditionRepresentation);
            }

            var conditionResponse = new ConditionResponse
            {
                Entry = conditions,
                Id = Uuid.Generate().Value.Split(":").Last(),
                ResourceType = "Bundle",
                Type = "collection"
            };
            return conditionResponse;
        }

        private async Task<ObservationResponse> FetchAllergiesData(DataRequest dataRequest,
            List<AllergyData> allergiesData, string patientName)
        {
            LogDataRequest(dataRequest);
            var uuid = Uuid.Generate().Value;
            var id = uuid.Split(":").Last();
            var observations = new List<ObservationRepresentation>();

            foreach (var allergyData in allergiesData)
            {
                if (!WithinRange(dataRequest.DateRange, allergyData.AllergyDate))
                {
                    continue;
                }

                var observationRepresentation = new ObservationRepresentation
                {
                    FullUrl = uuid,
                    Resource = new Resource(HiType.Observation, id, "final",
                        new Code("Allergies"),
                        new Subject(patientName),
                        allergyData.AllergyDate,
                        allergyData.Allergies, new List<Note> {new Note(allergyData.AllergyRemark)}),
                };
                observations.Add(observationRepresentation);
            }

            var observationResponse = new ObservationResponse
            {
                Entry = observations,
                Id = Uuid.Generate().Value.Split(":").Last(),
                ResourceType = "Bundle",
                Type = "collection"
            };
            return observationResponse;
        }

        private async Task<ObservationResponse> FetchAbdomenExaminationData(DataRequest dataRequest,
            List<AbdomenExaminationData> abdomenExaminationsData, string patientName)
        {
            LogDataRequest(dataRequest);
            var uuid = Uuid.Generate().Value;
            var id = uuid.Split(":").Last();
            var observations = new List<ObservationRepresentation>();

            foreach (var abdomenExaminationData in abdomenExaminationsData)
            {
                if (!WithinRange(dataRequest.DateRange, abdomenExaminationData.EffectiveDateTime))
                {
                    continue;
                }

                var observationRepresentation = new ObservationRepresentation
                {
                    FullUrl = uuid,
                    Resource = new Resource(HiType.Observation, id, "final",
                        new Code("Examination notes for Abdomen"), new Subject(patientName),
                        abdomenExaminationData.EffectiveDateTime, abdomenExaminationData.CAbdomen),
                };
                observations.Add(observationRepresentation);
            }

            var observationResponse = new ObservationResponse
            {
                Entry = observations,
                Id = Uuid.Generate().Value.Split(":").Last(),
                ResourceType = "Bundle",
                Type = "collection"
            };
            return observationResponse;
        }

        private async Task<ObservationResponse> FetchSurgeryHistoryData(DataRequest dataRequest,
            List<SurgeryHistory> surgeryHistories, string patientName)
        {
            LogDataRequest(dataRequest);
            var uuid = Uuid.Generate().Value;
            var id = uuid.Split(":").Last();
            var observations = new List<ObservationRepresentation>();
            foreach (var surgeryHistory in surgeryHistories)
            {
                if (!WithinRange(dataRequest.DateRange, surgeryHistory.SurgeryWhen))
                {
                    continue;
                }

                var observationRepresentation = new ObservationRepresentation
                {
                    FullUrl = uuid,
                    Resource = new Resource(HiType.Observation, id, "final",
                        new Code("Past history of surgery"), new Subject(patientName),
                        new List<Performer> {new Performer(surgeryHistory.HospitalDtls)},
                        surgeryHistory.SurgeryWhen, surgeryHistory.SurgeryDtls,
                        new List<Note> {new Note(surgeryHistory.SurgeryRemarks)}),
                };
                observations.Add(observationRepresentation);
            }

            var observationResponse = new ObservationResponse
            {
                Entry = observations,
                Id = Uuid.Generate().Value.Split(":").Last(),
                ResourceType = "Bundle",
                Type = "collection"
            };
            return observationResponse;
        }

        private async Task<ObservationResponse> FetchOralCavityExaminationsData(DataRequest dataRequest,
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

            var oralCavityExmResponse = new ObservationResponse
            {
                Entry = observations,
                Id = Uuid.Generate().Value.Split(":").Last(),
                ResourceType = "Bundle",
                Type = "collection"
            };
            return oralCavityExmResponse;
        }

        private async Task<PatientData> FetchPatientData(
            DataRequest dataRequest)
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

        private static void LogDataRequest(DataRequest request)
        {
            var ccList = JsonConvert.SerializeObject(request.CareContexts);
            var requestedHiTypes = string.Join(", ", request.HiType.Select(hiType => hiType.ToString()));
            Log.Information("Data request received." +
                            $" transactionId:{request.TransactionId} , " +
                            $"CareContexts:{ccList}, " +
                            $"HiTypes:{requestedHiTypes}," +
                            $" From date:{request.DateRange.From}," +
                            $" To date:{request.DateRange.To}, " +
                            $"CallbackUrl:{request.DataPushUrl}");
        }
    }
}