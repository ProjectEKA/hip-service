namespace In.ProjectEKA.HipService.User
{
    using System;
    using System.Threading.Tasks;
    using Common;
    using Gateway;
    using Hangfire;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json.Linq;

    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IBackgroundJobClient backgroundJob;
        private readonly GatewayClient gatewayClient;

        public UserController(IBackgroundJobClient backgroundJob, GatewayClient gatewayClient)
        {
            this.backgroundJob = backgroundJob;
            this.gatewayClient = gatewayClient;
        }

        [HttpPost(Constants.AUTH_CONFIRM)]
        public AcceptedResult authConfirm(JObject request)
        {
            backgroundJob.Enqueue(() => AuthFor(request));
            return Accepted();
        }

        [HttpPost(Constants.ON_AUTH_CONFIRM)]
        public AcceptedResult OnAuthConfirm(JObject request)
        {
            backgroundJob.Enqueue(() => OnAuthConfirmFor(request));
            return Accepted();
        }
        
        [NonAction]
        public async Task AuthFor(JObject request)
        {
            await gatewayClient.SendDataToGateway(Constants.PATH_AUTH_CONFIRM, request, "ncg")
                .ConfigureAwait(false);
        }

        [NonAction]
        public async Task OnAuthConfirmFor(JObject request)
        {
            Console.Write(request);
        }
        
    }
}