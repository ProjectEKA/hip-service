namespace In.ProjectEKA.HipService.Gateway
{
    using System;
    using System.Net.Http;
    using System.Net.Mime;
    using System.Text;
    using System.Threading.Tasks;
    using Common;
    using In.ProjectEKA.HipService.Gateway.Model;
    using Logger;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Optional;
    using static Common.HttpRequestHelper;

    public interface IGatewayClient
    {
        Task SendDataToGateway<T>(string urlPath, T response, string cmSuffix);
    }
    
    public class GatewayClient: IGatewayClient
    {
        private readonly GatewayConfiguration configuration;
        private readonly HttpClient httpClient;

        public GatewayClient(HttpClient httpClient, GatewayConfiguration gatewayConfiguration)
        {
            this.httpClient = httpClient;
            configuration = gatewayConfiguration;
        }

        public virtual async Task<Option<string>> Authenticate()
        {
            try
            {
                var json = JsonConvert.SerializeObject(new
                {
                    clientId = configuration.ClientId,
                    clientSecret = configuration.ClientSecret
                }, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    }
                });
                var message = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{configuration.Url}/{Constants.PATH_SESSIONS}"),
                    Method = HttpMethod.Post,
                    Content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json)
                };

                var responseMessage = await httpClient.SendAsync(message).ConfigureAwait(false);
                var response = await responseMessage.Content.ReadAsStringAsync();

                if (!responseMessage.IsSuccessStatusCode)
                {
                    var error = await responseMessage.Content.ReadAsStringAsync();
                    Log.Error(
                        $"Failure in getting the token with status code {responseMessage.StatusCode} {error}");
                    return Option.None<string>();
                }

                var definition = new {accessToken = "", tokenType = ""};
                var result = JsonConvert.DeserializeAnonymousType(response, definition);
                return Option.Some($"{result.tokenType} {result.accessToken}");
            }
            catch (Exception exception)
            {
                Log.Error(exception, exception.StackTrace);
                return Option.None<string>();
            }
        }

        public virtual async Task SendDataToGateway<T>(string urlPath, T response, string cmSuffix)
        {
            await PostTo(configuration.Url + urlPath, response, cmSuffix).ConfigureAwait(false);
        }

        private async Task PostTo<T>(string gatewayUrl, T representation, string cmSuffix)
        {
            try
            {
                var token = await Authenticate().ConfigureAwait(false);
                token.MatchSome(async accessToken =>
                {
                    try
                    {
                        await httpClient
                            .SendAsync(CreateHttpRequest(gatewayUrl, representation, accessToken, cmSuffix))
                            .ConfigureAwait(false);
                    }
                    catch (Exception exception)
                    {
                        Log.Error(exception, exception.StackTrace);
                    }
                });
                token.MatchNone(() => Log.Information("Data transfer notification to Gateway failed"));
            }
            catch (Exception exception)
            {
                Log.Error(exception, exception.StackTrace);
            }
        }

        public void SendDataToGateway(object pATH_CONSENT_ON_NOTIFY, GatewayRevokedConsentRepresentation gatewayRevokedConsentRepresentation, string id)
        {
            throw new NotImplementedException();
        }
    }
}