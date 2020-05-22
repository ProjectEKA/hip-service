namespace In.ProjectEKA.HipService.Gateway
{
    using System;
    using System.Net.Http;
    using System.Net.Mime;
    using System.Text;
    using System.Threading.Tasks;
    using Common;
    using Logger;
    using Microsoft.Net.Http.Headers;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class GatewayClient
    {
        private readonly HttpClient httpClient;
        private readonly CentralRegistryClient centralRegistryClient;
        private readonly GatewayConfiguration configuration;

        public GatewayClient(HttpClient httpClient,
            CentralRegistryClient centralRegistryClient, GatewayConfiguration gatewayConfiguration)
        {
            this.httpClient = httpClient;
            this.centralRegistryClient = centralRegistryClient;
            this.configuration = gatewayConfiguration;
        }

        public virtual async Task SendDataToGateway<T>(string urlPath, T response, string cmSuffix)
        {
            await PostTo(configuration.Url + urlPath, response, cmSuffix)
                .ConfigureAwait(false);
        }

        private async Task PostTo<T>(string gatewayUrl, T representation, string cmSuffix)
        {
            try
            {
                var token = await centralRegistryClient.Authenticate();
                token.MatchSome(async accessToken => await httpClient
                    .SendAsync(CreateHttpRequest(gatewayUrl, representation, accessToken, cmSuffix))
                    .ConfigureAwait(false));
                token.MatchNone(() => Log.Information("Data transfer notification to Gateway failed"));
            }
            catch (Exception exception)
            {
                Log.Error(exception, exception.StackTrace);
            }
        }

        private static HttpRequestMessage CreateHttpRequest<T>(string dataPushUrl,
            T content,
            string token,
            string cmSuffix)
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
                    {HeaderNames.Authorization, token},
                    {"X-CM-ID", cmSuffix}
                }
            };
        }
    }
}