namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Net.Http;
    using System.Net.Mime;
    using System.Text;
    using System.Threading.Tasks;
    using Common;
    using Logger;
    using Microsoft.Net.Http.Headers;
    using Model;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class DataFlowNotificationClient
    {
        private readonly HttpClient httpClient;
        private readonly CentralRegistryClient centralRegistryClient;

        public DataFlowNotificationClient(HttpClient httpClient, CentralRegistryClient centralRegistryClient)
        {
            this.httpClient = httpClient;
            this.centralRegistryClient = centralRegistryClient;
        }

        public virtual async void NotifyCm(string url, DataNotificationRequest dataNotificationRequest)
        {
            await PostTo(url, dataNotificationRequest);
        }

        private async Task PostTo(string url, DataNotificationRequest dataNotificationRequest)
        {
            try
            {
                var token = await centralRegistryClient.Authenticate();
                token.MatchSome(async accessToken => await httpClient
                    .SendAsync(CreateHttpRequest(dataNotificationRequest, accessToken, url))
                    .ConfigureAwait(false));
                token.MatchNone(() => Log.Information("Data transfer notification to CM failed"));
            }
            catch (Exception exception)
            {
                Log.Error(exception, exception.StackTrace);
            }
        }

        private static HttpRequestMessage CreateHttpRequest<T>(T content, string token, string url)
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
                RequestUri = new Uri($"{url}/health-information/notification"),
                Method = HttpMethod.Post,
                Content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json),
                Headers =
                {
                    {HeaderNames.Authorization, token}
                }
            };
        }
    }
}