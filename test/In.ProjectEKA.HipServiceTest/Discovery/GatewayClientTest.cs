using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using In.ProjectEKA.HipLibrary.Patient.Model;
using In.ProjectEKA.HipService.Common;
using In.ProjectEKA.HipService.Discovery;
using In.ProjectEKA.HipService.Gateway;
using In.ProjectEKA.HipService.Gateway.Model;
using Moq;
using Moq.Protected;
using Optional;
using Xunit;

namespace In.ProjectEKA.HipServiceTest.Discovery
{
    [Collection("Gateway Client Tests")]
    public class GatewayClientTest
    {
        [Fact]
        private void ShouldReturnDataToGateway()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(handlerMock.Object);
            var centralRegistryClient = new Mock<CentralRegistryClient>(MockBehavior.Strict, null, null);
            var gatewayConfiguration = new GatewayConfiguration {Url = "http://someUrl"};
            var expectedUri = new Uri("http://someUrl/care-contexts/on-discover");

            var patientEnquiryRepresentation = new PatientEnquiryRepresentation(
                "123",
                "Jack",
                new List<CareContextRepresentation>(),
                new List<string>()
            );

            var gatewayDiscoveryRepresentation = new GatewayDiscoveryRepresentation(
                patientEnquiryRepresentation,
                Guid.NewGuid(),
                DateTime.Now,
                "transactionId",
                null,
                new Resp("requestId")
            );

            var gatewayClient = new GatewayClient(httpClient, centralRegistryClient.Object, gatewayConfiguration);

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

            gatewayClient.SendDataToGateway(gatewayDiscoveryRepresentation, "ncg");

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
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(handlerMock.Object);
            var centralRegistryClient = new Mock<CentralRegistryClient>(MockBehavior.Strict, null, null);
            var gatewayConfiguration = new GatewayConfiguration {Url = "http://someUrl"};

            var patientEnquiryRepresentation = new PatientEnquiryRepresentation(
                "123",
                "Jack",
                new List<CareContextRepresentation>(),
                new List<string>()
            );

            var gatewayDiscoveryRepresentation = new GatewayDiscoveryRepresentation(
                patientEnquiryRepresentation,
                Guid.NewGuid(),
                DateTime.Now,
                "transactionId",
                null,
                new Resp("requestId")
            );

            var gatewayClient = new GatewayClient(httpClient, centralRegistryClient.Object, gatewayConfiguration);

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

            gatewayClient.SendDataToGateway(gatewayDiscoveryRepresentation, "ncg");

            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(0),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }
    }
}