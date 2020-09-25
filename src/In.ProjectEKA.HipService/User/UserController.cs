namespace In.ProjectEKA.HipService.User
{
    using System.Threading.Tasks;
    using Common;
    using Gateway;
    using Hangfire;
    using Logger;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json.Linq;
    using static Common.Constants;

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
                    Log.Information($"Access Token is: {authOnConfirmResponse.Auth.AccessToken}");
                if (authOnConfirmResponse.Auth.Patient != null)
                {
                    Log.Information("Patient Demographics Details:  "+$" Name: {authOnConfirmResponse.Auth.Patient.Name}, "+
                                    $"Id: {authOnConfirmResponse.Auth.Patient.Id}, "+
                                    $"Birth Year: {authOnConfirmResponse.Auth.Patient.YearOfBirth}, "+
                                    $"Gender: {authOnConfirmResponse.Auth.Patient.Gender}, ");
                    if (authOnConfirmResponse.Auth.Patient.Address != null)
                    {
                        Log.Information("Patient Address Details: "+
                                    $"District: {authOnConfirmResponse.Auth.Patient.Address.District}, "+
                                    $"State: {authOnConfirmResponse.Auth.Patient.Address.State}, "+ 
                                    $"Line: {authOnConfirmResponse.Auth.Patient.Address.Line}, "+
                                    $"Pincode: {authOnConfirmResponse.Auth.Patient.Address.PinCode}");
                    }
                }
            }
            else
                Log.Error($" Error Code:{authOnConfirmResponse.Error.Code}," +
                                      $" Error Message:{authOnConfirmResponse.Error.Message}.");
        }
    }
}