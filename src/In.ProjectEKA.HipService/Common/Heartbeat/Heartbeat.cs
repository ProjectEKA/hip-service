namespace In.ProjectEKA.HipService.Common.Heartbeat
{
    using System;
    using Model;

    public class Heartbeat
    {
        public static HeartbeatResponse GetHealthStatus()
        {
            return new HeartbeatResponse(DateTime.Now.ToUniversalTime(), HeartbeatStatus.UP.ToString(), null);
        }
    }
}