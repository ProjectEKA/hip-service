using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using HipLibrary.Patient.Model.Response;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Optional;

namespace In.ProjectEKA.DefaultHip.Link
{
    public class PatientVerification : IPatientVerification
    {
        private readonly IConfiguration configuration;
        private readonly HttpClient httpClient;

        public PatientVerification(IConfiguration configuration, HttpClient httpClient)
        {
            this.configuration = configuration;
            this.httpClient = httpClient;
        }
        
        public async Task<OtpMessage> SendTokenFor(Session session)
        {
            var urlConfig = configuration.GetSection("OtpService").Get<OtpService>();
            var uri = new Uri(urlConfig.BaseUrl + "/otp/link");
            var verifyMessage = await PostCallToOTPServer(uri
                , CreateHttpContent(session)).ConfigureAwait(false);
            return verifyMessage.ValueOr((OtpMessage) null);
        }
        
        public async Task<OtpMessage> Verify(string sessionId, string value)
        {
            var urlConfig = configuration.GetSection("OtpService").Get<OtpService>();
            var uri = new Uri(urlConfig.BaseUrl + "/otp/verify");
            var verifyOtpRequest = new OtpVerificationRequest(sessionId, value);
            var verifyMessage = await PostCallToOTPServer(
                uri,
                CreateHttpContent(verifyOtpRequest))
                .ConfigureAwait(false);
            return verifyMessage.ValueOr((OtpMessage) null);
        }
        
        private static HttpContent CreateHttpContent<T>(T content)
        {
            var json = JsonConvert.SerializeObject(content);  
            return new StringContent(json, Encoding.UTF8, "application/json");  
        }
        
        private async Task<Option<OtpMessage>> PostCallToOTPServer(Uri serverUrl, HttpContent content)
        {
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsync(serverUrl.ToString(), content);
            
                if (response.IsSuccessStatusCode)
                {
                    return Option.None<OtpMessage>();
                }

                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content;
                    using var reader = new StreamReader(await responseContent.ReadAsStreamAsync());
                    var result = await reader.ReadToEndAsync().ConfigureAwait(false);
                    var otpMessage = JsonConvert.DeserializeObject<OtpMessage>(result);
                    return Option.Some(otpMessage);
                }
                return Option.Some(new OtpMessage(ErrorCode.ServerInternalError.ToString(),
                    ErrorMessage.InternalServerError));
            }
            catch (Exception)
            {
                return Option.Some(new OtpMessage(ErrorCode.ServerInternalError.ToString(),
                    ErrorMessage.InternalServerError));
            }
        }
    }
}