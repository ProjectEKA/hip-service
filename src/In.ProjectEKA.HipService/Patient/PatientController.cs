namespace In.ProjectEKA.HipService.Patient
{
    using System;
    using System.Threading.Tasks;
    using Common;
    using Gateway;
    using Hangfire;
    using HipLibrary.Patient.Model;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Model;
    using Newtonsoft.Json.Linq;
    using User;

    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IBackgroundJobClient backgroundJob;
        private readonly GatewayClient gatewayClient;
        private readonly ILogger<UserController> logger;

        public PatientController(IBackgroundJobClient backgroundJob, GatewayClient gatewayClient, ILogger<UserController> logger)
        {
            this.backgroundJob = backgroundJob;
            this.gatewayClient = gatewayClient;
            this.logger = logger;
        }

        [HttpPost(Constants.PATH_PATIENT_PROFILE_SHARE)]
        public AcceptedResult PatientProfile(JObject request)
        {
            backgroundJob.Enqueue(() => ShareResponseFor(request));
            return Accepted();
        }
        
        [NonAction]
        public async Task ShareResponseFor(JObject request)
        {
            var patientProfileRequest = request.ToObject<PatientProfile>();
            logger.LogInformation($"Patient Details: {patientProfileRequest.Patient}");
            logger.LogInformation($"HIP Details: {patientProfileRequest.HipDetails}");
            var cmSuffix = patientProfileRequest.Patient.HealthId.Substring(
                patientProfileRequest.Patient.HealthId.LastIndexOf("@", StringComparison.Ordinal) + 1);
            var gatewayResponse = new PatientProfileAcknowledgementResponse(
                Guid.NewGuid(),
                DateTime.Now.ToUniversalTime(),
                new Acknowledgement(patientProfileRequest.Patient.HealthId, Status.SUCCESS), 
                new Resp(patientProfileRequest.RequestId), 
                null);
            await gatewayClient.SendDataToGateway(Constants.PATH_PATIENT_PROFILE_ON_SHARE, gatewayResponse, cmSuffix);
        }
    }
}