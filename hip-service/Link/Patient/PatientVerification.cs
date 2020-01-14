using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using hip_service.OTP;
using HipLibrary.Patient.Model.Response;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Optional;

namespace hip_service.Link.Patient
{
    public class PatientVerification: IPatientVerification
    {
        private readonly IConfiguration configuration;
        
        public PatientVerification(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        
        public async Task<OtpMessage> SendTokenFor(Session session)
        {
            var uri = new Uri(configuration.GetConnectionString("OTPGenerationConnection"));
            var verifyMessage = await PostCallToOTPServer(uri
                , CreateHttpContent(session));
            return verifyMessage.ValueOr((OtpMessage) null);
        }
        
        public async Task<OtpMessage> Verify(string sessionId, string value)
        {
            var uri = new Uri(configuration.GetConnectionString("OTPVerificationConnection"));
            var verifyOtpRequest = new OtpVerificationRequest(sessionId, value);
            var verifyMessage = await PostCallToOTPServer(uri
                , CreateHttpContent(verifyOtpRequest)).ConfigureAwait(false);
            return verifyMessage.ValueOr((OtpMessage) null);
        }
        
        private static HttpContent CreateHttpContent<T>(T content)  
        {
            var json = JsonConvert.SerializeObject(content);  
            return new StringContent(json, Encoding.UTF8, "application/json");  
        }
        
        private async Task<Option<OtpMessage>> PostCallToOTPServer (Uri serverUrl, HttpContent content)
        {
            var clientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };
            var httpClient = new HttpClient(clientHandler);
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsync(serverUrl.ToString()
                    , content);
            
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
                return Option.Some(new OtpMessage(ErrorCode.ServerInternalError.ToString()
                    , "Internal Server Error"));
            }
            catch (Exception)
            {
                return Option.Some(new OtpMessage(ErrorCode.ServerInternalError.ToString()
                    , "Internal Server Error"));
            }
        }
    }
}