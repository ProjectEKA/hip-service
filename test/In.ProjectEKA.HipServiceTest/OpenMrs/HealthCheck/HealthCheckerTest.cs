namespace In.ProjectEKA.HipServiceTest.OpenMrs
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net;
    using System.Threading.Tasks;
    using System.Threading;
    using System;
    using FluentAssertions;
    using In.ProjectEKA.HipService.OpenMrs.HealthCheck;
    using In.ProjectEKA.HipService.OpenMrs;
    using In.ProjectEKA.HipService;
    using Microsoft.AspNetCore.Hosting;
    using Moq.Protected;
    using Moq;
    using Xunit;

    [Collection("Health Check Client Tests")]
    public class HealthCheckerTest
    {
        private Mock<IHealthCheckStatus> healthCheckStatus;
        private Mock<IHealthCheckClient> healthCheckClient;
        private HealthChecker healthChecker;

        [Fact]
        private void ShouldNeverUpdateHealthCheckStatus()
        {
            Environment.SetEnvironmentVariable("HEALTH_CHECK_DURATION", "5000");

            healthCheckClient = new Mock<IHealthCheckClient>();
            healthCheckClient.Setup(x => x.CheckHealth())
                .Returns(Task.FromResult(new Dictionary<string, string>() { { "service", "Unhealthy" } }));
            healthCheckStatus = new Mock<IHealthCheckStatus>();
            healthChecker = new HealthChecker(healthCheckClient.Object, healthCheckStatus.Object);

            healthCheckStatus.Verify(x => x.AddStatus(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()), Times.Never);
        }

        [Fact]
        private void ShouldUpdateHealthCheckStatusOnce()
        {
            Environment.SetEnvironmentVariable("HEALTH_CHECK_DURATION", "5000");

            healthCheckClient = new Mock<IHealthCheckClient>();
            healthCheckClient.Setup(x => x.CheckHealth())
                .Returns(Task.FromResult(new Dictionary<string, string>() { { "service", "Unhealthy" } }));
            healthCheckStatus = new Mock<IHealthCheckStatus>();
            healthChecker = new HealthChecker(healthCheckClient.Object, healthCheckStatus.Object);

            var healthCheck = healthChecker.UpdateHealthStatus();

            healthCheckStatus.Verify(x => x.AddStatus(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()), Times.Exactly(1));
        }

        [Fact]
        private void ShouldUpdateHealthCheckStatusMoreThanOnce()
        {
            Environment.SetEnvironmentVariable("HEALTH_CHECK_DURATION", "10");

            healthCheckClient = new Mock<IHealthCheckClient>();
            healthCheckClient.Setup(x => x.CheckHealth())
                .Returns(Task.FromResult(new Dictionary<string, string>() { { "service", "Unhealthy" } }));
            healthCheckStatus = new Mock<IHealthCheckStatus>();
            healthChecker = new HealthChecker(healthCheckClient.Object, healthCheckStatus.Object);

            Thread.Sleep(25);

            healthCheckStatus.Verify(x => x.AddStatus(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()), Times.AtLeast(2));
        }

    }
}