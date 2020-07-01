using System;
using In.ProjectEKA.HipService.Common.Heartbeat.Model;

namespace In.ProjectEKA.HipService.Common.Heartbeat
{
    public class Heartbeat
    {
        
        public HeartbeatResponse GetHealthStatus()
        {
            var heartbeatResponse = new HeartbeatResponse(DateTime.Now, HeartbeatStatus.UP.ToString(),null);
            return heartbeatResponse;
        }
    }
}