namespace In.ProjectEKA.FHIRHipTest.Discovery
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using FHIRHip.DataFlow.Model;
    using FHIRHip.Discovery;
    using FHIRHip.Discovery.Model;
    using FluentAssertions;
    using HipLibrary.Patient.Model;
    using Microsoft.Extensions.Options;
    using Moq;
    using Moq.Protected;
    using Xunit;

    [Collection("Patient Repository Tests")]
    public class PatientMatchingRepositoryTest
    {
        private readonly IOptions<PatientConfiguration> patientConfiguration;

        public PatientMatchingRepositoryTest()
        {
            var dataFlow = new PatientConfiguration {BaseUrl = "http://localhost:8003",
                PathDiscovery = "/discovery/patients",
                PathLink = "/discovery/search/patient"};
            patientConfiguration = Options.Create(dataFlow);
        }
        
        [Fact]
        private async void ShouldReturnPatientsBasedOnExpression()
        {
            var expectedUri = new Uri("http://localhost:8003/discovery/patients");
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(handlerMock.Object);
            var phoneNumberIdentifier = new Identifier(IdentifierType.MOBILE, "+91-7777777777");
            var request = new DiscoveryRequest(
                new PatientEnquiry(string.Empty,
                    new List<Identifier> {phoneNumberIdentifier},
                    null,
                    string.Empty,
                    Gender.F,
                    (ushort) DateTime.Now.Year),
                string.Empty,
                "transactionId",
                DateTime.Now);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                })
                .Verifiable();
            
            var patientMatchingRepository = new PatientMatchingRepository(httpClient, patientConfiguration.Value);

            await patientMatchingRepository.Where(request);

            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(message => message.Method == HttpMethod.Post
                                                         && message.RequestUri == expectedUri),
                ItExpr.IsAny<CancellationToken>());
        }
    }
}
