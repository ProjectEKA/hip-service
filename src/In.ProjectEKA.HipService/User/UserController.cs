namespace In.ProjectEKA.HipService.User
{
    using System.Threading.Tasks;
    using Common;
    using Gateway;
    using Hangfire;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json.Linq;
    using static Common.Constants;

    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IBackgroundJobClient backgroundJob;
        private readonly GatewayClient gatewayClient;
        private readonly ILogger<UserController> logger;

        public UserController(IBackgroundJobClient backgroundJob, GatewayClient gatewayClient,
            ILogger<UserController> logger)
        {
            this.backgroundJob = backgroundJob;
            this.gatewayClient = gatewayClient;
            this.logger = logger;
        }

        [HttpPost(Constants.AUTH_CONFIRM)]
        public AcceptedResult authConfirm(
            [FromHeader(Name = CORRELATION_ID)] string correlationId,
            [FromBody] JObject request)
        {
            backgroundJob.Enqueue(() => AuthFor(request, correlationId));
            return Accepted();
        }

        [HttpPost(Constants.ON_AUTH_CONFIRM)]
        public AcceptedResult OnAuthConfirm(JObject response)
        {
            backgroundJob.Enqueue(() => OnAuthConfirmFor(response));
            return Accepted();
        }

        [NonAction]
        public async Task AuthFor(JObject request, string correlationId)
        {
            await gatewayClient.SendDataToGateway(Constants.PATH_AUTH_CONFIRM, request, "ncg", correlationId)
                .ConfigureAwait(false);
        }

        [NonAction]
        public async Task OnAuthConfirmFor(JObject response)
        {
            var authOnConfirmResponse = response.ToObject<AuthOnConfirmResponse>();
            if (authOnConfirmResponse.Error == null)
            {
                if (authOnConfirmResponse.Auth.AccessToken != null)
                    logger.LogInformation($"Access Token is: {authOnConfirmResponse.Auth.AccessToken}");
                if (authOnConfirmResponse.Auth.Patient != null)
                {
                    logger.LogInformation($"Patient Name is: {authOnConfirmResponse.Auth.Patient.Name}");
                    logger.LogInformation($"Patient Id is: {authOnConfirmResponse.Auth.Patient.Id}");
                    logger.LogInformation($"Patient Birth Year is: {authOnConfirmResponse.Auth.Patient.YearOfBirth}");
                    if (authOnConfirmResponse.Auth.Patient.Address != null)
                    {
                        logger.LogInformation(
                            $"Patient District is: {authOnConfirmResponse.Auth.Patient.Address.District}");
                        logger.LogInformation($"Patient State is: {authOnConfirmResponse.Auth.Patient.Address.State}");
                        logger.LogInformation($"Patient Line is: {authOnConfirmResponse.Auth.Patient.Address.Line}");
                        logger.LogInformation(
                            $"Patient Pincode is: {authOnConfirmResponse.Auth.Patient.Address.PinCode}");
                    }
                }
            }
            else
                logger.LogInformation($" Error Code:{authOnConfirmResponse.Error.Code}," +
                                      $" Error Message:{authOnConfirmResponse.Error.Message}.");
        }
    }
}