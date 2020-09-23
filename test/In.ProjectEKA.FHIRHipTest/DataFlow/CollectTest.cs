namespace In.ProjectEKA.FHIRHipTest.DataFlow
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using FHIRHip.DataFlow;
    using FHIRHip.DataFlow.Model;
    using FluentAssertions;
    using HipLibrary.Patient.Model;
    using Microsoft.Extensions.Options;
    using Moq;
    using Moq.Protected;
    using Optional.Unsafe;
    using Xunit;

    [Collection("Collect Tests")]
    public class CollectTest
    {
        private readonly IOptions<DataFlowConfiguration> dataFlowConfiguration;

        public CollectTest()
        {
            var dataFlow = new DataFlowConfiguration {Url = "http://localhost:8003/getData",
                                                        AuthUrl = "",
                                                        DataLinkTtlInMinutes = 0,
                                                        DataSizeLimitInMbs = 0,
                                                        IsAuthEnabled = false};
            dataFlowConfiguration = Options.Create(dataFlow);
        }
        
        [Fact]
        private async void ReturnCallDataFlowService()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(handlerMock.Object);
            var expectedUri = new Uri("http://localhost:8003/getData");

            const string consentId = "ConsentId";
            const string consentManagerId = "ConsentManagerId";
            var grantedContexts = new List<GrantedContext>
            {
                new GrantedContext("RVH1003", "BI-KTH-12.05.0024"),
                new GrantedContext("RVH1003", "NCP1008")
            };
            
            var dateRange = new DateRange("2017-12-01T15:43:00.818234", "2021-12-31T15:43:00.818234");
            var hiTypes = new List<HiType>
            {
                HiType.Condition,
                HiType.Observation,
                HiType.DiagnosticReport,
                HiType.MedicationRequest,
                HiType.DocumentReference,
                HiType.Prescription,
                HiType.DischargeSummary,
                HiType.OPConsultation
            };
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
            
            var dataRequest = new TraceableDataRequest(grantedContexts,
                dateRange,
                "/someUrl",
                hiTypes,
                "someTxnId",
                null,
                consentManagerId,
                consentId,
                "sometext",
                "someValue");
            var collect = new Collect(dataFlowConfiguration.Value, httpClient);
            var entries = await collect.CollectData(dataRequest);
            entries.ValueOrDefault().CareBundles.Should().NotBeNull();
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(message => message.Method == HttpMethod.Post
                                                         && message.RequestUri == expectedUri),
                ItExpr.IsAny<CancellationToken>());
        }
    }
}