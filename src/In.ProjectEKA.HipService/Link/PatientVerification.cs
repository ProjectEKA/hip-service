namespace In.ProjectEKA.HipService.Link
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.Mime;
    using System.Text;
    using System.Threading.Tasks;
    using Common;
    using HipLibrary.Patient.Model;
    using Logger;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;
    using Microsoft.Net.Http.Headers;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Optional;

    public class PatientVerification : IPatientVerification
    {
        private readonly HttpClient httpClient;
        private readonly IOptions<OtpServiceConfiguration> otpService;
        private readonly CentralRegistryClient centralRegistryClient;

        public PatientVerification(HttpClient httpClient,
                                   IOptions<OtpServiceConfiguration> otpService,
                                   CentralRegistryClient centralRegistryClient)
        {
            this.httpClient = httpClient;
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
            this.otpService = otpService;
            this.centralRegistryClient = centralRegistryClient;
        }

        public async Task<OtpMessage> SendTokenFor(Session session)
        { 
            var url = otpService.Value.BaseUrl + "/otp";
            var verifyMessage = await PostCallToOtpServer(url
                , session).ConfigureAwait(false);
            return verifyMessage.ValueOr((OtpMessage) null);
        }

        public async Task<OtpMessage> Verify(string sessionId, string value)
        {
            var url = new string($"{otpService.Value.BaseUrl}/otp/{sessionId}/verify");
            var verifyOtpRequest = new OtpVerificationRequest(value);
            var verifyMessage = await PostCallToOtpServer(url,
                verifyOtpRequest).ConfigureAwait(false);
            return verifyMessage.ValueOr((OtpMessage) null);
        }

        private async Task<Option<OtpMessage>> PostCallToOtpServer<T>(string serverUrl, T content)
        {
            var token = await centralRegistryClient.Authenticate();
            return await token.Map(async accessToken =>
                {
                    try
                    {
                        var response = await httpClient.SendAsync(CreateHttpRequest(serverUrl, content, accessToken));
                        using var reader = new StreamReader(await response.Content.ReadAsStreamAsync());
                        switch (response.StatusCode) {
                            case HttpStatusCode.OK:
                                return Option.None<OtpMessage>();
                            case HttpStatusCode.Unauthorized:
                                var statusResponse = new OtpMessage("404","Unauthorized request");
                                return Option.Some(statusResponse);
                            default:
                                var result = await reader.ReadToEndAsync().ConfigureAwait(false);
                                var otpMessage = JsonConvert.DeserializeObject<OtpMessage>(result);
                                Log.Information(otpMessage.Message);
                                return Option.Some(otpMessage);
                        }
                    }
                    catch (Exception exception)
                    {
                        Log.Fatal(exception, exception.StackTrace);
                        return Option.Some(new OtpMessage(ErrorCode.ServerInternalError.ToString(),
                            ErrorMessage.OtpServiceError));
                    }
                })
                .ValueOr(Task.FromResult(Option.Some(new OtpMessage(ErrorCode.ServerInternalError.ToString(),
                    ErrorMessage.OtpServiceError))));
        }
        
        private static HttpRequestMessage CreateHttpRequest<T>(string requestUrl, T content, string token)
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
                RequestUri = new Uri($"{requestUrl}"),
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