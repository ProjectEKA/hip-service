using System;
using In.ProjectEKA.HipService.Common.Heartbeat.Model;

namespace In.ProjectEKA.HipService.Common.Heartbeat
{
    public class Heartbeat
    {
        public static HeartbeatResponse GetHealthStatus()
        {
            return new HeartbeatResponse(DateTime.Now, HeartbeatStatus.UP.ToString(), null);
        }
    }
}