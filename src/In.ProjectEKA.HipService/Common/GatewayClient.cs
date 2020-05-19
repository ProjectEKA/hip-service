namespace In.ProjectEKA.HipService.Common
{
    using System;
    using System.Net.Http;
    using System.Net.Mime;
    using System.Text;
    using Common;
    using In.ProjectEKA.HipLibrary.Patient.Model;
    using Model;
    using Logger;
    using Microsoft.Net.Http.Headers;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Task = System.Threading.Tasks.Task;

    public class GatewayClient
    {
        private readonly HttpClient httpClient;
        private readonly CentralRegistryClient centralRegistryClient;

        public GatewayClient(HttpClient httpClient, CentralRegistryClient centralRegistryClient)
        {
            this.httpClient = httpClient;
            this.centralRegistryClient = centralRegistryClient;
        }

        public virtual async Task SendDataToGateway(String url, GatewayDiscoveryRepresentation discoveryResponse)
        {
            await PostTo(url, discoveryResponse);
        }

        private async Task PostTo(string gatewayUrl, GatewayDiscoveryRepresentation representation)
        {
            try
            {
                var token = await centralRegistryClient.Authenticate();
                token.MatchSome(async accessToken => await httpClient
                    .SendAsync(CreateHttpRequest(gatewayUrl, representation, accessToken))
                    .ConfigureAwait(false));
                token.MatchNone(() => Log.Information("Data transfer notification to Gateway failed"));
            }
            catch (Exception exception)
            {
                Log.Error(exception, exception.StackTrace);
            }
        }

        private static HttpRequestMessage CreateHttpRequest<T>(string dataPushUrl, T content, string token)
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
                RequestUri = new Uri($"{dataPushUrl}"),
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