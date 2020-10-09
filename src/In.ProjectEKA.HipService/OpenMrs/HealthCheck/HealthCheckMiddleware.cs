// You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using In.ProjectEKA.HipLibrary.Patient.Model;
using In.ProjectEKA.HipService.Common.Heartbeat.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using In.ProjectEKA.HipService.OpenMrs.HealthCheck;

public class HealthCheckMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHealthCheckStatus healthCheckStatus;
    private readonly HealthChecker healthChecker;

    public HealthCheckMiddleware(RequestDelegate next, IHealthCheckStatus inithealthCheckStatus,
        IServiceProvider serviceProvider)
    {
        healthChecker = (HealthChecker) serviceProvider.GetService(typeof(HealthChecker));
        healthCheckStatus = inithealthCheckStatus;
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        Dictionary<string, string> healthStatus = healthCheckStatus.GetStatus("health");
        bool healthy = true;
        String errorMessage = "";
        if (healthStatus != null)
        {
            foreach (var entry in healthStatus)
            {
                if (entry.Value != "Healthy")
                {
                    healthy = false;
                    errorMessage = entry.Key + " is " + entry.Value + "," + errorMessage;
                }
            }
        }

        if (!healthy)
        {
            httpContext.Response.StatusCode = 500;
            await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new HeartbeatResponse(
                DateTime.Now.ToUniversalTime(), HeartbeatStatus.DOWN.ToString(),
                new Error(ErrorCode.HeartBeat, errorMessage))));
        }
        else
        {
            await _next(httpContext);
        }
    }
}

public static class HealthCheckMiddlewareExtensions
{
    public static IApplicationBuilder UseHealthCheckMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<HealthCheckMiddleware>();
    }
}