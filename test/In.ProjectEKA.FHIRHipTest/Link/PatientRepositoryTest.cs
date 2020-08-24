namespace In.ProjectEKA.FHIRHipTest.Link
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using FHIRHip.Discovery.Model;
    using FHIRHip.Link;
    using FluentAssertions;
    using HipLibrary.Patient.Model;
    using Microsoft.Extensions.Options;
    using Moq;
    using Moq.Protected;
    using Xunit;

    [Collection("Patient Repository Tests")]
    public class PatientRepositoryTest
    {
        private readonly IOptions<PatientConfiguration> patientConfiguration;

        public PatientRepositoryTest()
        {
            var dataFlow = new PatientConfiguration
            {
                BaseUrl = "http://localhost:8003",
                PathDiscovery = "/discovery/patients",
                PathLink = "/discovery/search/patient"
            };
            patientConfiguration = Options.Create(dataFlow);
        }

        [Fact]
        private void ReturnPatient()
        {
            var expectedUri = new Uri("http://localhost:8003/discovery/search/patient");
            const string patientReferenceNumber = "RVH1002";
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(handlerMock.Object);
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
            var patientRepository = new PatientRepository(httpClient, patientConfiguration.Value);

            var patient =  patientRepository.PatientWith(patientReferenceNumber);

            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(message => message.Method == HttpMethod.Post
                                                         && message.RequestUri == expectedUri),
                ItExpr.IsAny<CancellationToken>());
        }
    }
}