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
    using Org.BouncyCastle.Crypto;
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
            var senderKeyPair = CryptoHelperInstance.GenerateKeyPair();
            var randomKeySender = CryptoHelperInstance.GenerateRandomKey();
            (await collect.CollectData(dataRequest))
                .Map(async entries =>
                {
                    var serializer = new FhirJsonSerializer(new SerializerSettings());
                    var healthRecordEntries = entries.Bundles
                        .Select(bundle => new Entry(
                             EncryptData(dataRequest.KeyMaterial,
                                 serializer.SerializeToString(bundle), senderKeyPair, randomKeySender),
                            "application/json",
                            "MD5"))
                        .ToList();
                    var keyMaterial = new KeyMaterial("",
                        "",
                        new KeyStructure("",
                            "",
                            CryptoHelperInstance.GetPublicKey(senderKeyPair)),
                        randomKeySender);                    
                    await SendDataToHiu(new DataResponse(dataRequest.TransactionId, healthRecordEntries, keyMaterial),
                        dataRequest.CallBackUrl);
                    return Task.CompletedTask;
                });
        }

        private static string EncryptData(HipLibrary.Patient.Model.KeyMaterial keyMaterial, string content,
            AsymmetricCipherKeyPair senderKeyPair, string randomKeySender)
        {
            var encryptedData = CryptoHelperInstance.EncryptData(keyMaterial.DhPublicKey.KeyValue,
                senderKeyPair, content, randomKeySender, keyMaterial.RandomKey);
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