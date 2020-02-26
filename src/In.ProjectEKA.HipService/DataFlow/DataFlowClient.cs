namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using HipLibrary.Patient;
    using Hl7.Fhir.Serialization;
    using Logger;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class DataFlowClient
    {
        private readonly ICollect collect;
        private readonly HttpClient httpClient;

        public DataFlowClient(ICollect collect, HttpClient httpClient)
        {
            this.collect = collect;
            this.httpClient = httpClient;
        }

        public async Task HandleMessagingQueueResult(HipLibrary.Patient.Model.DataRequest dataRequest)
        {
            (await collect.CollectData(dataRequest))
                .Map(async entries =>
                {
                    var serializer = new FhirJsonSerializer(new SerializerSettings());
                    var healthRecordEntries = entries.Bundles
                        .Select(bundle => new Entry(
                            serializer.SerializeToString(bundle),
                            "application/json",
                            "MD5"))
                        .ToList();
                    await SendDataToHiu(new DataResponse(dataRequest.TransactionId, healthRecordEntries),
                        dataRequest.CallBackUrl);
                    return Task.CompletedTask;
                });
        }

        private async Task SendDataToHiu(DataResponse dataResponse, string callBackUrl)
        {
            try
            {
                await httpClient.SendAsync(CreateHttpContent(callBackUrl, dataResponse))
                    .ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, exception.StackTrace);
            }
        }

        private static string GetAuthToken()
        {
            // TODO : Should be fetched from central registry
            return "AuthToken";
        }

        private static HttpRequestMessage CreateHttpContent<T>(string callBackUrl, T content)
        {
            var json = JsonConvert.SerializeObject(content, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });
            var httpRequestMessage = new HttpRequestMessage()
            {
                RequestUri = new Uri( $"{callBackUrl}/data/notification"),
                Method = HttpMethod.Post,
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
            };
            httpRequestMessage.Headers.Add("Authorization", GetAuthToken());
            return httpRequestMessage;
        }
    }
}