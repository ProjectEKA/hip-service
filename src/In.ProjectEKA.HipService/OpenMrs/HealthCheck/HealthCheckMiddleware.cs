// You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using In.ProjectEKA.HipService.OpenMrs.HealthCheck;

public class HealthCheckMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHealthCheckStatus healthCheckStatus;
    private readonly HealthChecker healthChecker;

    public HealthCheckMiddleware(RequestDelegate next, IHealthCheckStatus inithealthCheckStatus, IServiceProvider serviceProvider)
    {
        healthChecker = (HealthChecker)serviceProvider.GetService(typeof(HealthChecker));
        healthCheckStatus = inithealthCheckStatus;
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        Dictionary<string, string> healthStatus = healthCheckStatus.GetStatus("health");
        bool healthy = true;
        if (healthStatus != null)
        {
            foreach (var entry in healthStatus)
            {
                if (entry.Value != "Healthy")
                {
                    healthy = false;
                    break;
                }
            }
        }
        if (!healthy)
        {
            httpContext.Response.StatusCode = 500;
            await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(healthStatus));
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