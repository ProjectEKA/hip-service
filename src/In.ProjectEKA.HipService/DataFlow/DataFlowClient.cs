namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using CryptoHelper;
    using Logger;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Optional;
    using Task = System.Threading.Tasks.Task;

    public class DataFlowClient
    {
        private readonly HttpClient httpClient;
        private readonly ICryptoHelper cryptoHelper;

        public DataFlowClient()
        {
        }

        public DataFlowClient(HttpClient httpClient, ICryptoHelper cryptoHelper)
        {
            this.httpClient = httpClient;
            this.cryptoHelper = cryptoHelper;
        }

        public virtual void SendDataToHiu(HipLibrary.Patient.Model.DataRequest dataRequest,
            Option<IEnumerable<Entry>> data,
            KeyMaterial keyMaterial)
        {
            data.Map(async entries => 
                await PostTo(dataRequest.CallBackUrl, new DataResponse(dataRequest.TransactionId, entries, keyMaterial)));
        }

        private async Task PostTo(string callBackUrl, DataResponse dataResponse)
        {
            try
            {
                await httpClient.SendAsync(CreateHttpRequest(callBackUrl, dataResponse))
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

        private static HttpRequestMessage CreateHttpRequest<T>(string callBackUrl, T content)
        {
            var json = JsonConvert.SerializeObject(content, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });
            return new HttpRequestMessage
            {
                RequestUri = new Uri( $"{callBackUrl}/data/notification"),
                Method = HttpMethod.Post,
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
                Headers =
                {
                    { "Authorization", GetAuthToken()}
                }
            };
        }
    }
}