namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using CryptoHelper;
    using HipLibrary.Patient;
    using Hl7.Fhir.Serialization;
    using Logger;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Org.BouncyCastle.Crypto;

    public class DataFlowClient
    {
        private readonly ICollect collect;
        private readonly HttpClient httpClient;
        private readonly ICryptoHelper cryptoHelper;

        public DataFlowClient(ICollect collect, HttpClient httpClient, ICryptoHelper cryptoHelper)
        {
            this.collect = collect;
            this.httpClient = httpClient;
            this.cryptoHelper = cryptoHelper;
        }

        public async Task HandleMessagingQueueResult(HipLibrary.Patient.Model.DataRequest dataRequest)
        {
            var senderKeyPair = cryptoHelper.GenerateKeyPair(dataRequest.KeyMaterial.Curve,
                dataRequest.KeyMaterial.CryptoAlg);
            var randomKeySender = cryptoHelper.GenerateRandomKey();
            (await collect.CollectData(dataRequest))
                .Map(async entries =>
                {
                    var serializer = new FhirJsonSerializer(new SerializerSettings());
                    var healthRecordEntries = entries.Bundles
                        .Select(bundle => new Entry(
                             EncryptData(dataRequest.KeyMaterial, serializer.SerializeToString(bundle), 
                                 senderKeyPair, randomKeySender),
                            "application/json",
                            "MD5"))
                        .ToList();
                    var keyMaterial = new KeyMaterial(dataRequest.KeyMaterial.CryptoAlg, dataRequest.KeyMaterial.Curve,
                        new KeyStructure("", "", cryptoHelper.GetPublicKey(senderKeyPair)),
                        randomKeySender);                    
                    await SendDataToHiu(new DataResponse(dataRequest.TransactionId, healthRecordEntries, keyMaterial),
                        dataRequest.CallBackUrl).ConfigureAwait(false);
                    return Task.CompletedTask;
                });
        }

        private string EncryptData(HipLibrary.Patient.Model.KeyMaterial keyMaterial, string content,
            AsymmetricCipherKeyPair senderKeyPair, string randomKeySender)
        {
            var encryptedData = cryptoHelper.EncryptData(keyMaterial.DhPublicKey.KeyValue, senderKeyPair,
                content, randomKeySender, keyMaterial.Nonce, keyMaterial.Curve,
                keyMaterial.CryptoAlg);
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