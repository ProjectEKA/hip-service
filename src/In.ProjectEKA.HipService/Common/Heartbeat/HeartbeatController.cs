namespace In.ProjectEKA.HipService.Common.Heartbeat
{
    using Microsoft.AspNetCore.Mvc;
    using Model;
    using static Constants;

    // TODO: This is dummy implementation and actual implementation has to be done by respective HIPs
    [ApiController]
    public class HeartbeatController : ControllerBase
    {
        [HttpGet(PATH_HEART_BEAT)]
        public HeartbeatResponse GetProvidersByName()
        {
            return Heartbeat.GetHealthStatus();
        }
    }
}