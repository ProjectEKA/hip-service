namespace In.ProjectEKA.HipService.Link
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.Mime;
    using System.Text;
    using System.Threading.Tasks;
    using Logger;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Optional;

    public class PatientVerification : IPatientVerification
    {
        private readonly HttpClient httpClient;
        private readonly IOptions<OtpServiceConfiguration> otpService;

        public PatientVerification(HttpClient httpClient, IOptions<OtpServiceConfiguration> otpService)
        {
            this.httpClient = httpClient;
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
            this.otpService = otpService;
        }

        public async Task<OtpMessage> SendTokenFor(Session session)
        {
            var urlConfig = otpService.Value.BaseUrl;
            var uri = new Uri(urlConfig + "/otp");
            var verifyMessage = await PostCallToOtpServer(uri
                , CreateHttpContent(session)).ConfigureAwait(false);
            return verifyMessage.ValueOr((OtpMessage) null);
        }

        public async Task<OtpMessage> Verify(string sessionId, string value)
        {
            var urlConfig = otpService.Value.BaseUrl;
            var uri = new Uri($"{urlConfig}/otp/{sessionId}/verify");
            var verifyOtpRequest = new OtpVerificationRequest(value);
            var verifyMessage = await PostCallToOtpServer(uri,
                CreateHttpContent(verifyOtpRequest)).ConfigureAwait(false);
            return verifyMessage.ValueOr((OtpMessage) null);
        }

        private static HttpContent CreateHttpContent<T>(T content)
        {
            var json = JsonConvert.SerializeObject(content);
            return new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
        }

        private async Task<Option<OtpMessage>> PostCallToOtpServer(Uri serverUrl, HttpContent content)
        {
            try
            {
                var response = await httpClient.PostAsync(serverUrl.ToString(), content);
                if (response.IsSuccessStatusCode)
                    return Option.None<OtpMessage>();

                var responseContent = response.Content;
                using var reader = new StreamReader(await responseContent.ReadAsStreamAsync());
                var result = await reader.ReadToEndAsync().ConfigureAwait(false);
                var otpMessage = JsonConvert.DeserializeObject<OtpMessage>(result);
                Log.Information(otpMessage.Message);
                return Option.Some(otpMessage);
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, exception.StackTrace);
                return Option.Some(new OtpMessage(ResponseType.InternalServerError,
                    ErrorMessage.OtpServiceError));
            }
        }
    }
}