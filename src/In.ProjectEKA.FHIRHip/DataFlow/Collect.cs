namespace In.ProjectEKA.DefaultHip.DataFlow
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Mime;
    using System.Text;
    using System.Threading.Tasks;
    using HipLibrary.Patient;
    using HipLibrary.Patient.Model;
    using Hl7.Fhir.Model;
    using Hl7.Fhir.Serialization;
    using Hl7.Fhir.Utility;
    using Model;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Optional;
    using Patient;
    using Serilog;

    public class Collect : ICollect
    {
        private readonly string HipSystemDataUrl;
        private readonly HttpClient HttpClient;

        public Collect(string hipSystemDataUrl, HttpClient httpClient)
        {
            HipSystemDataUrl = hipSystemDataUrl;
            HttpClient = httpClient;
        }

        public async Task<Option<Entries>> CollectData(DataRequest dataRequest)
        {
            var bundles = new List<CareBundle>();
            var patientData = await FindPatientsData(dataRequest);
            var careContextReferences = patientData.Keys.ToList();
            foreach (var careContextReference in careContextReferences)
                foreach (var result in patientData.GetOrDefault(careContextReference))
                {
                    Log.Information($"Returning file: {result}");
                    var fjp = new FhirJsonParser();
                    bundles.Add(new CareBundle(careContextReference, fjp.Parse<Bundle>(result)));
                }

            var entries = new Entries(bundles);
            return Option.Some(entries);
        }

        private async Task<Dictionary<string, List<string>>> FindPatientsData(DataRequest request)
        {
            // LogDataRequest(request);
            
            var patientReferenceNumber = request.CareContexts.First().PatientReference;
            var careContexts = request.CareContexts.Select(careContext => careContext.CareContextReference).ToList();
            var dataResponse = await GetPatientsData(new NetworkDataRequest(patientReferenceNumber,
                                                             careContexts,
                                                             request.DateRange,
                                                             request.HiType));
            var structuredData = new Dictionary<string, List<string>>();
            return dataResponse.Map(content =>
            {
                foreach (var result in content.Results)
                {
                    if (structuredData.ContainsKey(result.CareContext))
                    {
                        structuredData[result.CareContext].Add(result.Data);
                    }
                    else
                    {
                        structuredData.Add(result.CareContext, new List<string> {result.Data});
                    }
                }
                return structuredData;
            }).ValueOr(structuredData);
        }

        private async Task<Option<NetworkDataResponse>> GetPatientsData(NetworkDataRequest networkDataRequest)
        {
            try
            {
                var json = JsonConvert.SerializeObject(networkDataRequest, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    }
                });
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri($"{HipSystemDataUrl}"))
                {
                    Content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json)
                };
                var response = await HttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                    return Option.None<NetworkDataResponse>();
                
                var responseContent = response.Content;
                using var reader = new StreamReader(await responseContent.ReadAsStreamAsync());
                var result = await reader.ReadToEndAsync().ConfigureAwait(false);
                return Option.Some(JsonConvert.DeserializeObject<NetworkDataResponse>(result));
            }
            catch (Exception exception)
            {
                Log.Error(exception, exception.StackTrace);
                return Option.None<NetworkDataResponse>();
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