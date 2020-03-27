namespace In.ProjectEKA.HipService.Common
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Logger;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Optional;

    public class CentralRegistryClient
    {
        private readonly HttpClient httpClient;
        private readonly CentralRegistryConfiguration centralRegistryConfiguration;

        public CentralRegistryClient(HttpClient httpClient, CentralRegistryConfiguration centralRegistryConfiguration)
        {
            this.httpClient = httpClient;
            this.centralRegistryConfiguration = centralRegistryConfiguration;
        }

        public virtual async Task<Option<string>> Authenticate()
        {
            try
            {
                var responseMessage = await httpClient.SendAsync(CreateHttpRequest(centralRegistryConfiguration.Url,
                        new
                        {
                            clientId = centralRegistryConfiguration.ClientId,
                            clientSecret = centralRegistryConfiguration.ClientSecret,
                            grantType = "password"
                        }))
                    .ConfigureAwait(false);
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
                RequestUri = new Uri($"{callBackUrl}/api/1.0/sessions"),
                Method = HttpMethod.Post,
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
            };
        }
    }
}