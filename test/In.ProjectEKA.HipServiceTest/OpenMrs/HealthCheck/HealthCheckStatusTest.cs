namespace In.ProjectEKA.HipServiceTest.OpenMrs
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net;
    using System.Threading.Tasks;
    using System.Threading;
    using System;
    using FluentAssertions;
    using In.ProjectEKA.HipService;
    using In.ProjectEKA.HipService.OpenMrs.HealthCheck;
    using Moq.Protected;
    using Moq;
    using Xunit;
    using Microsoft.AspNetCore.Hosting;

    [Collection("Health Check Status Tests")]
    public class HealthCheckStatusTest
    {

        [Fact]
        private void ShouldAddHealthCheckStatusWhenAddStatusIsInvoked()
        {
            Dictionary<string, string> status = new Dictionary<string, string> { { "Service1", "Unhealthy" }, { "Service2", "Healthy" } };
            HealthCheckStatus healthCheckStatus = new HealthCheckStatus();
            healthCheckStatus.AddStatus("health", status);

            Dictionary<string, string> result = healthCheckStatus.GetStatus("health");

            result.Should().Equal(status);
        }

        [Fact]
        private void ShouldUpdateHealthCheckStatusWhenStatusAlreadyExists()
        {
            Dictionary<string, string> status1 = new Dictionary<string, string> { { "Service1", "Unhealthy" }, { "Service2", "Healthy" } };
            HealthCheckStatus healthCheckStatus = new HealthCheckStatus();
            healthCheckStatus.AddStatus("health", status1);
            Dictionary<string, string> status2 = new Dictionary<string, string> { { "Service1", "Healthy" }, { "Service2", "Healthy" } };
            healthCheckStatus.AddStatus("health", status2);

            Dictionary<string, string> result = healthCheckStatus.GetStatus("health");

            result.Should().Equal(status2);
        }

        [Fact]
        private void ShouldReturnNullWhenHealthCheckStatusIsEmpty()
        {
            HealthCheckStatus healthCheckStatus = new HealthCheckStatus();

            Dictionary<string, string> result = healthCheckStatus.GetStatus("health");

            result.Should().BeNull();
        }
    }
}