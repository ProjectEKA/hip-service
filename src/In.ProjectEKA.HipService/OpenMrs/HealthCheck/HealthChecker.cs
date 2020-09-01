using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using In.ProjectEKA.HipService.OpenMrs.HealthCheck;

    public class HealthChecker {
        private IHealthCheckClient healthCheckClient;
        private Timer timer;

        private IHealthCheckStatus healthCheckStatus;

        public HealthChecker (IHealthCheckClient initHealthCheckClient, IHealthCheckStatus inithealthCheckStatus) {
            healthCheckClient = initHealthCheckClient;
            healthCheckStatus = inithealthCheckStatus;
            timer = new Timer(Convert.ToInt32(Environment.GetEnvironmentVariable("HEALTH_CHECK_DURATION")));
            timer.Elapsed += async ( sender, e ) => await UpdateHealthStatus();
            timer.Start();
        }

        public async Task UpdateHealthStatus () {
            Dictionary<string, string> result = await healthCheckClient.CheckHealth(); 
            healthCheckStatus.AddStatus("health",result);
        }

    }
