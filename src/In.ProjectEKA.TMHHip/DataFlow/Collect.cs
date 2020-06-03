using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Utility;
using In.ProjectEKA.HipLibrary.Patient;
using In.ProjectEKA.TMHHip.DataFlow.Model;
using Newtonsoft.Json;
using Optional.Linq;
using Code = In.ProjectEKA.TMHHip.DataFlow.Model.Code;
using Resource = In.ProjectEKA.TMHHip.DataFlow.Model.Resource;

namespace In.ProjectEKA.TMHHip.DataFlow
{
    using System.Threading.Tasks;
    using HipLibrary.Patient.Model;
    using Optional;
    using Serilog;
    using JsonSerializer = System.Text.Json.JsonSerializer;

    public class Collect : ICollect
    {
        private readonly HttpClient _client;
        private readonly IPatientRepository _patientRepository;

        public Collect(HttpClient client, IPatientRepository patientRepository)
        {
            this._client = client;
            this._patientRepository = patientRepository;
        }

        public async Task<Option<Entries>> CollectData(DataRequest dataRequest)
        {
            var bundles = new List<CareBundle>();
            var patientData = FindPatientData(dataRequest).Result;
            var careContextReferences = patientData.Keys.ToList();

            var parser = new FhirJsonParser(new ParserSettings
            {
                AcceptUnknownMembers = true,
                AllowUnrecognizedEnums = true
            });
            foreach (var careContextReference in careContextReferences)
            {
                foreach (var result in patientData.GetOrDefault(careContextReference))
                {
                    Log.Information($"Returning file: {result}");
                    var content = JsonConvert.SerializeObject(result);
                    bundles.Add(new CareBundle(careContextReference, parser.Parse<Bundle>(content)));
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

        private async Task<Dictionary<string, List<HealthObservationRepresentation>>> FindPatientData(
            DataRequest dataRequest)
        {
            LogDataRequest(dataRequest);
            var caseId = dataRequest.CareContexts.First().PatientReference;
            var patient = _patientRepository.PatientWith(caseId);
            var patientName = patient.Select(patient => patient.Name).ValueOr("");

            var careContextsAndListOfDataFiles = new Dictionary<string, List<HealthObservationRepresentation>>();
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

            var response = await _client.SendAsync(request);
            await using var responseStream = await response.Content.ReadAsStreamAsync();
            var clinicalNotes = await JsonSerializer.DeserializeAsync<IEnumerable<ClinicalNote>>(responseStream);
            var uuid = Uuid.Generate().Value;
            var id = uuid.Split(":").Last();
            var healthObservationRepresentations = new List<HealthObservationRepresentation>();
            foreach (var clinicalNote in clinicalNotes)
            {
                if (!WithinRange(dataRequest.DateRange, clinicalNote.CreatedDate)) continue;

                healthObservationRepresentations.Add(
                    new HealthObservationRepresentation
                    {
                        FullUrl = uuid,
                        Resource = new Resource(HiType.Observation, id, "final", new Code("Clinical notes"),
                            new Subject(patientName), new List<Performer> {new Performer(clinicalNote.UserName)},
                            clinicalNote.CreatedDate,
                            clinicalNote.Note),
                    }
                );
            }

            careContextsAndListOfDataFiles.Add(caseId, healthObservationRepresentations);
            return careContextsAndListOfDataFiles;
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