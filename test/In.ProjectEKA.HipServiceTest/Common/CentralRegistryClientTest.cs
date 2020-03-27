namespace In.ProjectEKA.HipServiceTest.Common
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Builder;
    using FluentAssertions;
    using HipService.Common;
    using Moq;
    using Moq.Protected;
    using Newtonsoft.Json;
    using Xunit;

    public class CentralRegistryClientTest
    {
        [Fact]
        private async void ShouldReturnAccessToken()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(handlerMock.Object);
            const string centralRegistryRootUrl = "https://root/central-registry";
            var expectedUri = new Uri($"{centralRegistryRootUrl}/api/1.0/sessions");
            var centralRegistryConfiguration = new CentralRegistryConfiguration
            {
                Url = centralRegistryRootUrl,
                ClientId = TestBuilder.RandomString(),
                ClientSecret = TestBuilder.RandomString()
            };
            var response = JsonConvert.SerializeObject(new {tokenType = "bearer", accessToken = "token"});
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(response, Encoding.UTF8, "application/json")
                })
                .Verifiable();

            var registryClient = new CentralRegistryClient(httpClient, centralRegistryConfiguration);

            var result = await registryClient.Authenticate();

            result.HasValue.Should().BeTrue();
            result.MatchSome(token => token.Should().BeEquivalentTo("bearer token"));
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(message => message.Method == HttpMethod.Post
                                                         && message.RequestUri == expectedUri),
                ItExpr.IsAny<CancellationToken>());
        }

        [Theory]
        [InlineData(HttpStatusCode.Unauthorized)]
        [InlineData(HttpStatusCode.InternalServerError)]
        private async void ShouldNotReturnTokenWhenErrorResponse(HttpStatusCode statusCode)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(handlerMock.Object);
            const string centralRegistryRootUrl = "https://root/central-registry";
            var expectedUri = new Uri($"{centralRegistryRootUrl}/api/1.0/sessions");
            var centralRegistryConfiguration = new CentralRegistryConfiguration
            {
                Url = centralRegistryRootUrl,
                ClientId = TestBuilder.RandomString(),
                ClientSecret = TestBuilder.RandomString()
            };
            var response = JsonConvert.SerializeObject(new {error = "some failure happened"});
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(response, Encoding.UTF8, "application/json")
                })
                .Verifiable();

            var registryClient = new CentralRegistryClient(httpClient, centralRegistryConfiguration);

            var result = await registryClient.Authenticate();

            result.HasValue.Should().BeFalse();
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(message => message.Method == HttpMethod.Post
                                                         && message.RequestUri == expectedUri),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        private async void ShouldNotReturnTokenWhenAnyExceptionHappened()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(handlerMock.Object);
            var registryClient = new CentralRegistryClient(httpClient, null);

            var result = await registryClient.Authenticate();

            result.HasValue.Should().BeFalse();
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(0),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }
    }
}