namespace In.ProjectEKA.HipServiceTest.OpenMrs.HealthCheck
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net;
    using System.Threading.Tasks;
    using System.Threading;
    using System;
    using FluentAssertions;
    using In.ProjectEKA.HipService.OpenMrs;
    using Moq.Protected;
    using Moq;
    using Xunit;
    using In.ProjectEKA.HipService.OpenMrs.HealthCheck;

    [Collection("Health Check Client Tests")]
    public class OpenMrsHealthCheckClientTest
    {
        private Mock<IHealthCheckClient> healthCheckClient;

        [Fact]
        private async void ShouldReturnDictionaryWhenCheckingForHealth()
        {
            healthCheckClient = new Mock<IHealthCheckClient>();
            healthCheckClient.Setup(x => x.CheckHealth())
                .Returns(Task.FromResult(new Dictionary<string, string>()));

            var result = await healthCheckClient.Object.CheckHealth();

            result.GetType().Should().Be(new Dictionary<string, string>().GetType());
        }

        [Fact]
        private async void ShouldReturnServiceAsHealthyWhenServiceHasStatus200()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(handlerMock.Object);
            var openmrsConfiguration = new OpenMrsConfiguration
            {
                Url = "https://someurl/openmrs/",
                Username = "someusername",
                Password = "somepassword"
            };
            var openmrsClient = new OpenMrsClient(httpClient, openmrsConfiguration);
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
            OpenMrsHealthCheckClient openMrsHealthCheckClient = new OpenMrsHealthCheckClient(new Dictionary<string, string> { { "Service", "path/to/resource" }
            }, openmrsClient);

            var result = await openMrsHealthCheckClient.CheckHealth();

            result["Service"].Should().Be("Healthy");
        }

        [Fact]
        private async void ShouldReturnServiceAsUnhealthyWhenServiceHasStatus500()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(handlerMock.Object);
            var openmrsConfiguration = new OpenMrsConfiguration
            {
                Url = "https://someurl/openmrs/",
                Username = "someusername",
                Password = "somepassword"
            };
            var openmrsClient = new OpenMrsClient(httpClient, openmrsConfiguration);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                })
                .Verifiable();
            OpenMrsHealthCheckClient openMrsHealthCheckClient = new OpenMrsHealthCheckClient(new Dictionary<string, string> { { "Service", "path/to/resource" }
            }, openmrsClient);

            var result = await openMrsHealthCheckClient.CheckHealth();

            result["Service"].Should().Be("Unhealthy");
        }

        [Fact]
        private async void ShouldReturnServiceAsUnhealthyWhenRequestThrowsException()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(handlerMock.Object);
            var openmrsConfiguration = new OpenMrsConfiguration
            {
                Url = "https://someurl/openmrs/",
                Username = "someusername",
                Password = "somepassword"
            };
            var openmrsClient = new OpenMrsClient(httpClient, openmrsConfiguration);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new Exception("Throw some exception"))
                .Verifiable();
            OpenMrsHealthCheckClient openMrsHealthCheckClient = new OpenMrsHealthCheckClient(new Dictionary<string, string> { { "Service", "path/to/resource" }
            }, openmrsClient);

            var result = await openMrsHealthCheckClient.CheckHealth();

            result["Service"].Should().Be("Unhealthy");
        }
    }
}