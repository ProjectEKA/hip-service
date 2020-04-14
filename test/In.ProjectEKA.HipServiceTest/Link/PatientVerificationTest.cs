using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit;

namespace In.ProjectEKA.HipServiceTest.Link
{
    using Bogus;
    using Builder;
    using FluentAssertions;
    using HipService.Common;
    using HipService.Link;
    using Optional;

    [Collection("Patient Verification Tests")]
    public class PatientVerificationTest
    {
        private readonly IOptions<OtpServiceConfiguration> otpServiceConfigurations;

        public PatientVerificationTest()
        {
            var otpService = new OtpServiceConfiguration {BaseUrl = "http://localhost:5000"};
            otpServiceConfigurations = Options.Create(otpService);
        }

        [Fact]
        private async void ReturnFailureOnOtpCreation()
        {
            const string centralRegistryRootUrl = "https://root/central-registry";
            var centralRegistryClient = new Mock<CentralRegistryClient>(MockBehavior.Strict, null, null);
            var session = new Session(TestBuilder.Faker().Random.Hash()
                , new Communication(CommunicationMode.MOBILE, "+91666666666666"));
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
            centralRegistryClient.Setup(client => client.Authenticate()).ReturnsAsync(Option.Some("Something"));
            var patientVerification = new PatientVerification(httpClient, otpServiceConfigurations,
                centralRegistryClient.Object);

            var result = await patientVerification.SendTokenFor(session);

            result.Should().NotBeNull();
            result.Message.Should().BeEquivalentTo("Unable to create Otp");
        }

        [Fact]
        private async void ReturnSuccessOnOtpCreation()
        {
            const string centralRegistryRootUrl = "https://root/central-registry";
            var centralRegistryClient = new Mock<CentralRegistryClient>(MockBehavior.Strict, null, null);
            var session = new Session(TestBuilder.Faker().Random.Hash()
                , new Communication(CommunicationMode.MOBILE, "+91666666666666"));
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
            centralRegistryClient.Setup(client => client.Authenticate()).ReturnsAsync(Option.Some("Something"));
            var patientVerification = new PatientVerification(httpClient, otpServiceConfigurations,
                centralRegistryClient.Object);

            var result = await patientVerification.SendTokenFor(session);

            result.Should().BeNull();
        }
    }
}