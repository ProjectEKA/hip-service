using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using In.ProjectEKA.DefaultHip.DataFlow;
using In.ProjectEKA.HipService.DataFlow;
using In.ProjectEKA.HipServiceTest.DataFlow.Builder;
using Moq;
using Moq.Protected;
using Xunit;
using System.Linq.Expressions;

namespace In.ProjectEKA.HipServiceTest.DataFlow
{
    [Collection("Queue Listener Tests")]
    public class DataFlowClientTest
    {
        private readonly Collect collect = new Collect("observation.json");

        [Fact]
        private async Task ReturnSuccessOnDataFlow()
        {
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
                })
                .Verifiable();
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost:8003"),
            };
            var dataRequest = TestBuilder.DataRequest().Generate().Build();

            var dataFlowClient = new DataFlowClient(collect, httpClient);

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