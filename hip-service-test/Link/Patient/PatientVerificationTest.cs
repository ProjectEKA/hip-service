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
        private IConfiguration configuration;
        public PatientVerificationTest()
        {
            configuration = TestBuilder.GetIConfigurationRoot();
        }

        [Fact]
        private async void ReturnFailureOnOtpCreation()
        {
            var session = new Session(TestBuilder.Faker().Random.Hash()
                , new Communication(CommunicationMode.MOBILE,"+91666666666666"));
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("{'code':1,'message':'Unable to create Otp'}"),
                })
                .Verifiable();
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(configuration.GetConnectionString("OTPGenerationConnection")),
            };
            var patientVerification = new PatientVerification(configuration,httpClient);
            
            var result = await patientVerification.SendTokenFor(session);
            
            result.Should().NotBeNull();
            result.Message.Should().BeEquivalentTo("Unable to create Otp");
        }
        
        [Fact]
        private async void ReturnSuccessOnOtpCreation()
        {
            var session = new Session(TestBuilder.Faker().Random.Hash()
                , new Communication(CommunicationMode.MOBILE,"+91666666666666"));
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{'code':1,'message':'Otp created'}"),
                })
                .Verifiable();
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(configuration.GetConnectionString("OTPGenerationConnection")),
            };
            var patientVerification = new PatientVerification(configuration,httpClient);
            
            var result = await patientVerification.SendTokenFor(session);
            
            result.Should().BeNull();        }
    }
}