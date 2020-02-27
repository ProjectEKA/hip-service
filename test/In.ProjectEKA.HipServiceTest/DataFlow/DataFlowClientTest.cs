namespace In.ProjectEKA.HipServiceTest.DataFlow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using In.ProjectEKA.HipService.DataFlow;
    using Builder;
    using Moq;
    using Moq.Protected;
    using Optional;
    using Xunit;

    [Collection("Data Flow Client Tests")]
    public class DataFlowClientTest
    {
        [Fact]
        private void ShouldReturnDataComponent()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(handlerMock.Object);
            var dataRequest = TestBuilder.DataRequest(TestBuilder.Faker().Random.Hash());
            var content = TestBuilder.Faker().Random.String();
            var checksum = TestBuilder.Faker().Random.Hash();
            var entries = Option.Some(new List<Entry>
                {
                    new Entry(content, "application/json", checksum, null)
                }
                .AsEnumerable());
            var expectedUri = new Uri("http://callback/data/notification");
            var dataFlowClient = new DataFlowClient(httpClient);

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
            dataFlowClient.SendDataToHiu(dataRequest, entries);

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