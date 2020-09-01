using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
namespace In.ProjectEKA.HipService.OpenMrs.HealthCheck
{
    public interface IHealthCheckClient
    {
        Task<Dictionary<string, string>> CheckHealth();
    }
}