namespace In.ProjectEKA.HipServiceTest.DataFlow
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Builder;
    using HipLibrary.Patient;
    using HipLibrary.Patient.Model;
    using Hl7.Fhir.Model;
    using In.ProjectEKA.HipService.DataFlow;
    using Moq;
    using Moq.Protected;
    using Optional;
    using Xunit;
    using Task = System.Threading.Tasks.Task;

    [Collection("Queue Listener Tests")]
    public class DataFlowClientTest
    {

        [Fact]
        private async Task ReturnSuccessOnDataFlow()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var collect = new Mock<ICollect>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                })
                .Verifiable();
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost:8003")
            };
            var dataRequest = TestBuilder.DataRequest().Generate().Build();
            collect.Setup(a => a.CollectData(dataRequest))
                .Returns(Task.FromResult(Option.Some(new Entries(new List<Bundle>()))));

            var dataFlowClient = new DataFlowClient(collect.Object, httpClient);

            await dataFlowClient.HandleMessagingQueueResult(dataRequest);
            var expectedUri = new Uri("http://localhost:8003/data/notification");

            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri == expectedUri),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}