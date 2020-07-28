namespace In.ProjectEKA.HipService.User
{
    using System;
    using System.Threading.Tasks;
    using Common;
    using Hangfire;
    using Link;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json.Linq;

    [Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IBackgroundJobClient backgroundJob;

        public UserController(IBackgroundJobClient backgroundJob)
        {
            this.backgroundJob = backgroundJob;
        }

        [HttpPost(Constants.ON_AUTH_CONFIRM)]
        public AcceptedResult LinkFor(JObject request)
        {
            backgroundJob.Enqueue(() => LinkPatient(request));
            return Accepted();
        }
        
        [NonAction]
        public async Task LinkPatient(JObject request)
        {
            Console.Write(request);
        }
        
    }
}