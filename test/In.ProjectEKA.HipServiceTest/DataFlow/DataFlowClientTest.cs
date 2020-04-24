namespace In.ProjectEKA.HipServiceTest.DataFlow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Mime;
    using System.Threading;
    using System.Threading.Tasks;
    using In.ProjectEKA.HipService.DataFlow;
    using Builder;
    using FluentAssertions;
    using HipService.Common;
    using In.ProjectEKA.HipService.DataFlow.Model;
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
            var id = "ConsentManagerId";
            const string centralRegistryRootUrl = "https://root/central-registry";
            var centralRegistryClient = new Mock<CentralRegistryClient>(MockBehavior.Strict, null, null);
            var dataFlowNotificationClient = new Mock<DataFlowNotificationClient>(MockBehavior.Strict, null, null);
            var centralRegistryConfiguration = new CentralRegistryConfiguration
            {
                Url = centralRegistryRootUrl,
                ClientId = "10000005",
                ClientSecret = TestBuilder.RandomString()
            };
            var transactionId = "transactionId";
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(handlerMock.Object);
            var dataRequest = TestBuilder.DataRequest(transactionId);
            var content = TestBuilder.Faker().Random.String();
            var checksum = TestBuilder.Faker().Random.Hash();
            var dataNotificationRequest = TestBuilder.DataNotificationRequest(transactionId);

            var entries = new List<Entry>
                {
                    new Entry(content, MediaTypeNames.Application.Json, checksum, null,"careContextReference")
                }
                .AsEnumerable();
            var expectedUri = new Uri("http://callback/data/notification");
            var dataFlowClient = new DataFlowClient(httpClient,
                centralRegistryClient.Object,
                dataFlowNotificationClient.Object,
                centralRegistryConfiguration);

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
            centralRegistryClient.Setup(client => client.GetUrlFor(id))
                .ReturnsAsync(Option.Some("http://localhost:8000"));
            dataFlowNotificationClient.Setup(client =>
                    client.NotifyCm("http://localhost:8000",
                        It.IsAny<DataNotificationRequest>()))
                .Callback((string url, DataNotificationRequest request) =>
                    {
                        dataNotificationRequest.Should().NotBeNull();
                        dataNotificationRequest.Notifier.Id.Should().Be(request.Notifier.Id);
                        dataNotificationRequest.Notifier.Type.Should().Be(request.Notifier.Type);
                        dataNotificationRequest.TransactionId.Should().Be(request.TransactionId);
                        dataNotificationRequest.StatusNotification.HipId.Should().Be(request.StatusNotification.HipId);
                        dataNotificationRequest.StatusNotification.SessionStatus.Should().Be(request.StatusNotification.SessionStatus);
                        url.Should().Be("http://localhost:8000");
                    });

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
            var id = "ConsentManagerId";
            const string centralRegistryRootUrl = "https://root/central-registry";
            var centralRegistryClient = new Mock<CentralRegistryClient>(MockBehavior.Strict, null, null);
            var dataFlowNotificationClient = new Mock<DataFlowNotificationClient>(MockBehavior.Strict, null, null);
            var centralRegistryConfiguration = new CentralRegistryConfiguration
            {
                Url = centralRegistryRootUrl,
                ClientId = id,
                ClientSecret = TestBuilder.RandomString()
            };
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(handlerMock.Object);
            var dataRequest = TestBuilder.DataRequest(TestBuilder.Faker().Random.Hash());
            var entries = new List<Entry>().AsEnumerable();
            var dataFlowClient = new DataFlowClient(httpClient,
                centralRegistryClient.Object,
                dataFlowNotificationClient.Object,
                centralRegistryConfiguration);

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
            centralRegistryClient.Setup(client => client.GetUrlFor(id)).ReturnsAsync(Option.None<string>());

            dataFlowClient.SendDataToHiu(dataRequest, entries, null);

            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(0),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }
    }
}