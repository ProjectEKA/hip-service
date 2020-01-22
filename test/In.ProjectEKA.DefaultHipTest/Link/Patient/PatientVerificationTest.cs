using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using In.ProjectEKA.DefaultHip.Link;
using In.ProjectEKA.DefaultHipTest.Link.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Moq;
using Moq.Protected;
using Xunit;

namespace In.ProjectEKA.DefaultHipTest.Link.Patient
{

    [Collection("Patient Verification Tests")]
    public class PatientVerificationTest
    {
        private readonly Mock<IConfiguration> configurationMock = new Mock<IConfiguration>();

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
                BaseAddress = new Uri("http://localhost:5000/otp/link"),
            };
            var mockConfSection = new Mock<IConfigurationSection>();
            mockConfSection.SetupGet(m => m[It.Is<string>(s => s == "OTPGenerationConnection")])
                .Returns("http://localhost:5000/otp/link");
            configurationMock.Setup(x => x.GetSection(It.Is<string>(s=>s == "ConnectionStrings")))
                .Returns(mockConfSection.Object);

            var patientVerification = new PatientVerification(configurationMock.Object,httpClient);
            
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
                BaseAddress = new Uri("http://localhost:5000/otp/link"),
            };
            var mockConfSection = new Mock<IConfigurationSection>();
            mockConfSection.SetupGet(m => m[It.Is<string>(s => s == "OTPGenerationConnection")])
                .Returns("http://localhost:5000/otp/link");
            configurationMock.Setup(x => x.GetSection(It.Is<string>(s=>s == "ConnectionStrings")))
                .Returns(mockConfSection.Object);
            
            var patientVerification = new PatientVerification(configurationMock.Object,httpClient);
            
            var result = await patientVerification.SendTokenFor(session);
            
            result.Should().BeNull();        }
    }
}