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
                        if (tmhPatientData.ClinicalNotes != null)
                        {
                            var observationResponse =
                                FindObservationData(dataRequest, tmhPatientData.ClinicalNotes, patientName)
                                    .Result;
                            var serializeObject = JsonConvert.SerializeObject(observationResponse,
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
                        break;
                    case HiType.DiagnosticReport:
                        break;
                    case HiType.Medication:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            var entries = new Entries(careBundles);
            return Option.Some(entries);
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
                "yyyy-MM-ddTHH:mm:ss.ffff", "yyyy-MM-ddTHH:mm:ss.fffff", "yyyy-MM-ddTHH:mm:ss.ffffff",
                "dd/MM/yyyy", "dd/MM/yyyy hh:mm:ss", "dd/MM/yyyy hh:mm:ss tt", "dd/MM/yyyyTHH:mm:ss.fffzzz"
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
                var medicationRepresentation = new MedicationRepresentation
                {
                    FullUrl = uuidMedication,
                    Resource = new MedicationResource
                        {Id = medicationId, ResourceType = HiType.Medication, Code = new Code(prescription.Medicine)}
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

        private async Task<ObservationResponse> FindObservationData(DataRequest dataRequest,
            List<ClinicalNote> clinicalNotes, string patientName)
        {
            LogDataRequest(dataRequest);
            var uuid = Uuid.Generate().Value;
            var id = uuid.Split(":").Last();
            var list = new List<ObservationRepresentation>();

            foreach (var clinicalNote in clinicalNotes)
            {
                if (!WithinRange(dataRequest.DateRange, clinicalNote.CreatedDate))
                {
                    continue;
                }

                var observationRepresentation = new ObservationRepresentation
                {
                    FullUrl = uuid,
                    Resource = new Resource(HiType.Observation, id, "final", new Code("Clinical notes"),
                        new Subject(patientName), new List<Performer> {new Performer(clinicalNote.UserName)},
                        clinicalNote.CreatedDate,
                        clinicalNote.Note),
                };
                list.Add(observationRepresentation);
            }

            var observationResponse = new ObservationResponse
            {
                Entry = list,
                Id = Uuid.Generate().Value.Split(":").Last(),
                ResourceType = "Bundle",
                Type = "collection"
            };
            return observationResponse;
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