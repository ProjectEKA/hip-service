namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using HipLibrary.Patient;
    using Hl7.Fhir.Serialization;
    using Logger;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using CryptoHelperInstance = CryptoHelper.CryptoHelper;
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
                             EncryptData(dataRequest.KeyMaterial,serializer.SerializeToString(bundle)),
                            "application/json",
                            "MD5"))
                        .ToList();
                    await SendDataToHiu(new DataResponse(dataRequest.TransactionId, healthRecordEntries),
                        dataRequest.CallBackUrl);
                    return Task.CompletedTask;
                });
        }

        private static string EncryptData(HipLibrary.Patient.Model.KeyMaterial keyMaterial, string content)
        {
            var encryptedData = CryptoHelperInstance.EncryptData(keyMaterial.DhPublicKey.KeyValue, content);
            return encryptedData;
        }

        private async Task SendDataToHiu(DataResponse dataResponse, string callBackUrl)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", GetAuthToken());
                await httpClient.PostAsync($"{callBackUrl}/data/notification", CreateHttpContent(dataResponse))
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
            return Convert.ToBase64String(Encoding.UTF8.GetBytes("AuthToken"));
        }

        private static HttpContent CreateHttpContent<T>(T content)
        {
            var json = JsonConvert.SerializeObject(content, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}