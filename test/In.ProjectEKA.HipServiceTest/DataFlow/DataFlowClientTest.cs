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
        private async Task ShouldReturnDataComponent()
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
            var httpClient = new HttpClient(handlerMock.Object);
            var dataRequest = TestBuilder.DataRequest(TestBuilder.Faker().Random.Hash());
            collect.Setup(iCollect => iCollect.CollectData(dataRequest))
                .ReturnsAsync(Option.Some(new Entries(new List<Bundle>())));
            var dataFlowClient = new DataFlowClient(collect.Object,
                httpClient);

            await dataFlowClient.HandleMessagingQueueResult(dataRequest);

            var expectedUri = new Uri("http://callback/data/notification");
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(message => message.Method == HttpMethod.Post
                                                         && message.RequestUri == expectedUri),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}