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
    using HipService.Common;
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
            var centralRegistryClient = new Mock<CentralRegistryClient>(MockBehavior.Strict, null, null);
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(handlerMock.Object);
            var dataRequest = TestBuilder.DataRequest(TestBuilder.Faker().Random.Hash());
            var content = TestBuilder.Faker().Random.String();
            var checksum = TestBuilder.Faker().Random.Hash();
            var entries = new List<Entry>
                {
                    new Entry(content, "application/json", checksum, null)
                }
                .AsEnumerable();
            var expectedUri = new Uri("http://callback/data/notification");
            var dataFlowClient = new DataFlowClient(httpClient, centralRegistryClient.Object);

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
            centralRegistryClient.Setup(client => client.Authenticate()).ReturnsAsync(Option.Some("Something"));

            dataFlowClient.SendDataToHiu(dataRequest, entries, null);

            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(message => message.Method == HttpMethod.Post
                                                         && message.RequestUri == expectedUri),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        private void ShouldNotPostDataIfAuthenticationWithCentralRegistryFailed()
        {
            var centralRegistryClient = new Mock<CentralRegistryClient>(MockBehavior.Strict, null, null);
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(handlerMock.Object);
            var dataRequest = TestBuilder.DataRequest(TestBuilder.Faker().Random.Hash());
            var entries = new List<Entry>().AsEnumerable();
            var dataFlowClient = new DataFlowClient(httpClient, centralRegistryClient.Object);

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
            centralRegistryClient.Setup(client => client.Authenticate()).ReturnsAsync(Option.None<string>());

            dataFlowClient.SendDataToHiu(dataRequest, entries, null);

            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(0),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }
    }
}