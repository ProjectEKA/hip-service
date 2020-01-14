using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using hip_service.Link.Patient;
using hip_service.OTP;
using hip_service_test.Link.Builder;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Xunit;

namespace hip_service_test.Link.Patient
{
    [Collection("Patient Verification Tests")]
    public class PatientVerificationTest
    {
        private readonly IConfiguration configuration;
        private readonly PatientVerification patientVerification;
        public PatientVerificationTest()
        {
            configuration = TestBuilder.GetIConfigurationRoot();
            patientVerification = new PatientVerification(configuration);
        }

        [Fact]
        private async void ReturnFailureOnOtpCreation()
        {
            var handlerMock = new Mock<HttpClientHandler>(MockBehavior.Strict);
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{'ReferenceType':1001,'value':'Otp created'}"),
                })
                .Verifiable();
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost.com:5000/otp/link")
            };
            var session = new Session(TestBuilder.Faker().Random.Hash()
                , new Communication(CommunicationMode.MOBILE,"+91666666666666"));
            
            var result = await patientVerification.SendTokenFor(session);
            
            result.Should().NotBeNull();
        }
    }
}