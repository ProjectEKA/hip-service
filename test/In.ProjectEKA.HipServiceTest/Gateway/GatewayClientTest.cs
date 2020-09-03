using Hl7.Fhir.Model;

namespace In.ProjectEKA.HipServiceTest.Gateway
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Mime;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Common.Builder;
    using FluentAssertions;
    using HipLibrary.Patient.Model;
    using HipService.Gateway;
    using HipService.Gateway.Model;
    using Moq;
    using Moq.Protected;
    using Newtonsoft.Json;
    using Xunit;
    using static HipService.Common.Constants;

    [Collection("Gateway Client Tests")]
    public class GatewayClientTest
    {
        [Fact]
        private void ShouldReturnDataToGateway()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(handlerMock.Object);
            var gatewayConfiguration = new GatewayConfiguration {Url = "http://someUrl"};
            var authenticationUri = new Uri($"{gatewayConfiguration.Url}/{PATH_SESSIONS}");
            var expectedUri = new Uri($"{gatewayConfiguration.Url}{PATH_ON_DISCOVER}");
            var patientEnquiryRepresentation = new PatientEnquiryRepresentation(
                "123",
                "Jack",
                new List<CareContextRepresentation>(),
                new List<string>());
            var gatewayDiscoveryRepresentation = new GatewayDiscoveryRepresentation(
                patientEnquiryRepresentation,
                Guid.NewGuid(),
                DateTime.Now,
                "transactionId",
                null,
                new DiscoveryResponse("requestId", HttpStatusCode.OK, "uselessMessage"));
            var gatewayClient = new GatewayClient(httpClient, gatewayConfiguration);
            var definition = new {accessToken = "Whatever", tokenType = "Bearer"};
            var result = JsonConvert.SerializeObject(definition);
            var correlationId = Uuid.Generate().ToString();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsInOrder(() => Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(result)
                }), () => Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                }));

            gatewayClient.SendDataToGateway(PATH_ON_DISCOVER, gatewayDiscoveryRepresentation, "ncg", correlationId);

            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(message => message.Method == HttpMethod.Post
                                                         && message.RequestUri == expectedUri),
                ItExpr.IsAny<CancellationToken>());
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(message => message.Method == HttpMethod.Post
                                                         && message.RequestUri == authenticationUri),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        private void ShouldNotPostDataIfAuthenticationWithGatewayFailed()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(handlerMock.Object);
            var gatewayConfiguration = new GatewayConfiguration {Url = "http://someUrl"};
            var authenticationUri = new Uri($"{gatewayConfiguration.Url}/{PATH_SESSIONS}");
            var patientEnquiryRepresentation = new PatientEnquiryRepresentation(
                "123",
                "Jack",
                new List<CareContextRepresentation>(),
                new List<string>());
            var gatewayDiscoveryRepresentation = new GatewayDiscoveryRepresentation(
                patientEnquiryRepresentation,
                Guid.NewGuid(),
                DateTime.Now,
                "transactionId",
                null,
                new DiscoveryResponse("requestId", HttpStatusCode.OK, "uselessMessage"));
            var gatewayClient = new GatewayClient(httpClient, gatewayConfiguration);
            var correlationId = Uuid.Generate().ToString();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadGateway
                })
                .Verifiable();

            gatewayClient.SendDataToGateway(PATH_ON_DISCOVER, gatewayDiscoveryRepresentation, "ncg", correlationId);

            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(message => message.Method == HttpMethod.Post
                                                         && message.RequestUri == authenticationUri),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        private async void ShouldReturnAccessToken()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(handlerMock.Object);
            const string rootRul = "http://someUrl";
            var expectedUri = new Uri($"{rootRul}/{PATH_SESSIONS}");
            var correlationId = Uuid.Generate().ToString();
            var configuration = new GatewayConfiguration
            {
                Url = rootRul,
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
                    Content = new StringContent(response, Encoding.UTF8, MediaTypeNames.Application.Json)
                })
                .Verifiable();

            var client = new GatewayClient(httpClient, configuration);

            var result = await client.Authenticate(correlationId);

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
            const string centralRegistryRootUrl = "http://someUrl";
            var expectedUri = new Uri($"{centralRegistryRootUrl}/{PATH_SESSIONS}");
            var configuration = new GatewayConfiguration
            {
                Url = centralRegistryRootUrl,
                ClientId = TestBuilder.RandomString(),
                ClientSecret = TestBuilder.RandomString()
            };
            var response = JsonConvert.SerializeObject(new {error = "some failure happened"});
            var correlationId = Uuid.Generate().ToString();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(response, Encoding.UTF8, MediaTypeNames.Application.Json)
                })
                .Verifiable();

            var client = new GatewayClient(httpClient, configuration);

            var result = await client.Authenticate(correlationId);

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
            var gatewayClient = new GatewayClient(httpClient, null);
            var correlationId = Uuid.Generate().ToString();

            var result = await gatewayClient.Authenticate(correlationId);

            result.HasValue.Should().BeFalse();
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(0),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }
    }
}