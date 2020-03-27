namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using Common;
    using Logger;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Task = System.Threading.Tasks.Task;

    public class DataFlowClient
    {
        private readonly HttpClient httpClient;
        private readonly CentralRegistryClient centralRegistryClient;

        public DataFlowClient(HttpClient httpClient, CentralRegistryClient centralRegistryClient)
        {
            this.httpClient = httpClient;
            this.centralRegistryClient = centralRegistryClient;
        }

        public virtual async void SendDataToHiu(HipLibrary.Patient.Model.DataRequest dataRequest,
            IEnumerable<Entry> data,
            KeyMaterial keyMaterial)
        {
            await PostTo(dataRequest.CallBackUrl, new DataResponse(dataRequest.TransactionId, data, keyMaterial));
        }

        private async Task PostTo(string callBackUrl, DataResponse dataResponse)
        {
            try
            {
                var token = await centralRegistryClient.Authenticate();
                token.MatchSome(async accessToken => await httpClient
                    .SendAsync(CreateHttpRequest(callBackUrl, dataResponse, accessToken))
                    .ConfigureAwait(false));
                token.MatchNone(() => Log.Information("Did not post data to HIU"));
            }
            catch (Exception exception)
            {
                Log.Error(exception, exception.StackTrace);
            }
        }

        private static HttpRequestMessage CreateHttpRequest<T>(string callBackUrl, T content, string token)
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
                RequestUri = new Uri($"{callBackUrl}/data/notification"),
                Method = HttpMethod.Post,
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
                Headers =
                {
                    {"Authorization", token}
                }
            };
        }
    }
}