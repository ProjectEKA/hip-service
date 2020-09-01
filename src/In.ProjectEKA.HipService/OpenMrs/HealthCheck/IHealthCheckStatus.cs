using System.Collections.Generic;
namespace In.ProjectEKA.HipService.OpenMrs.HealthCheck
{
    public interface IHealthCheckStatus
    {
        void AddStatus(string key, Dictionary<string, string> value);
        Dictionary<string, string> GetStatus(string key);
    }
}