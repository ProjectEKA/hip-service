namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using Logger;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Optional;
    using Task = System.Threading.Tasks.Task;

    public class DataFlowClient: IDataFlowClient
    {
        private readonly HttpClient httpClient;

        public DataFlowClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public void SendDataToHiu(HipLibrary.Patient.Model.DataRequest dataRequest, Option<IEnumerable<Entry>> data)
        {
            data.Map(async entries => 
                await PostTo(dataRequest.CallBackUrl, new DataResponse(dataRequest.TransactionId, entries)));
        }

        private async Task PostTo(string callBackUrl, DataResponse dataResponse)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", GetAuthToken());
                var hiuUrl = $"{callBackUrl}/data/notification";
                await httpClient
                    .PostAsync(hiuUrl, CreateHttpContent(dataResponse))
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