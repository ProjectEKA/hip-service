using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Utility;
using In.ProjectEKA.HipLibrary.Patient;
using In.ProjectEKA.HipLibrary.Patient.Model;
using In.ProjectEKA.TMHHip.DataFlow.Model;
using Newtonsoft.Json;
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
            this.patientRepository = patientRepository;
        }

        public async Task<Option<Entries>> CollectData(DataRequest dataRequest)
        {
            var bundles = new List<CareBundle>();
            var tmhPatientData = FetchPatientData(dataRequest).Result;
            var caseId = dataRequest.CareContexts.First().PatientReference;
            var patient = patientRepository.PatientWith(caseId);
            var patientName = patient.Select(patientObj => patientObj.Name).ValueOr("");

            foreach (var hiType in dataRequest.HiType)
            {
                switch (hiType)
                {
                    case HiType.Observation:
                        bundles.AddRange(FindObservationData(dataRequest, tmhPatientData.ClinicalNotes, patientName).Result);
                        break;
                    case HiType.MedicationRequest:
                        bundles.AddRange(FindMedicationRequestData(dataRequest, tmhPatientData.Prescriptions, patientName).Result);
                        break;
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

            var entries = new Entries(bundles);
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
                "yyyy-MM-dd", "yyyy-MM-dd hh:mm:ss", "yyyy-MM-dd hh:mm:ss tt", "yyyy-MM-ddTHH:mm:ss.fffzzz",
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

        private async Task<List<CareBundle>> FindMedicationRequestData(DataRequest dataRequest,
            List<Prescription> prescriptions, string patientName)
        {
            LogDataRequest(dataRequest);
            var caseId = dataRequest.CareContexts.First().PatientReference;
            var parser = new FhirJsonParser(new ParserSettings
            {
                AcceptUnknownMembers = true,
                AllowUnrecognizedEnums = true
            });
            var careBundles = new List<CareBundle>();
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
                        Intent = "active",
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

                var prescriptionRepresentation = new PrescriptionRepresentation
                {
                    MedicationRepresentation = medicationRepresentation,
                    MedicationRequestRepresentation = medicationRequestRepresentation
                };
                var content = JsonConvert.SerializeObject(prescriptionRepresentation);
                var careBundle = new CareBundle(caseId, parser.Parse<Bundle>(content));
                careBundles.Add(careBundle);
            }

            return careBundles;
        }

        private async Task<List<CareBundle>> FindObservationData(DataRequest dataRequest,
            List<ClinicalNote> clinicalNotes, string patientName)
        {
            LogDataRequest(dataRequest);
            var caseId = dataRequest.CareContexts.First().PatientReference;
            var uuid = Uuid.Generate().Value;
            var id = uuid.Split(":").Last();
            var careBundles = new List<CareBundle>();
            var parser = new FhirJsonParser(new ParserSettings
            {
                AcceptUnknownMembers = true,
                AllowUnrecognizedEnums = true
            });
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
                var content = JsonConvert.SerializeObject(observationRepresentation);
                var careBundle = new CareBundle(caseId, parser.Parse<Bundle>(content));
                careBundles.Add(careBundle);
            }

            return careBundles;
        }

        private async Task<PatientData> FetchPatientData(
            DataRequest dataRequest)
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

            var response = await client.SendAsync(request);
            await using var responseStream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<PatientData>(responseStream);
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