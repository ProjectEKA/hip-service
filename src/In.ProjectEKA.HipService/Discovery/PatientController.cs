namespace In.ProjectEKA.HipService.Discovery
{
    using System;
    using System.Threading.Tasks;
    using Common;
    using Gateway;
    using Gateway.Model;
    using Hangfire;
    using HipLibrary.Patient.Model;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using static Common.Constants;

    [Authorize]
    [ApiController]
    public class CareContextDiscoveryController : Controller
    {
        private readonly IBackgroundJobClient backgroundJob;
        private readonly GatewayClient gatewayClient;
        private readonly ILogger<CareContextDiscoveryController> logger;
        private readonly PatientDiscovery patientDiscovery;

        public CareContextDiscoveryController(PatientDiscovery patientDiscovery,
            GatewayClient gatewayClient,
            IBackgroundJobClient backgroundJob, ILogger<CareContextDiscoveryController> logger)
        {
            this.patientDiscovery = patientDiscovery;
            this.gatewayClient = gatewayClient;
            this.backgroundJob = backgroundJob;
            this.logger = logger;
        }

        [HttpPost(PATH_CARE_CONTEXTS_DISCOVER)]
        public AcceptedResult DiscoverPatientCareContexts(
            [FromHeader(Name = CORRELATION_ID)] string correlationId, 
            [FromBody] DiscoveryRequest request)
        {
            logger.LogInformation(LogEvents.Discovery, "discovery request received for {@Patient} with {RequestId}",
                request.Patient, request.RequestId);
            backgroundJob.Enqueue(() => GetPatientCareContext(request, correlationId));
            return Accepted();
        }

        // ReSharper disable once MemberCanBePrivate.Global
        [NonAction]
        public async Task GetPatientCareContext(DiscoveryRequest request, string correlationId)
        {
            try
            {
                var (response, error) = await patientDiscovery.PatientFor(request);
                var patientId = request.Patient.Id;
                var cmSuffix = patientId.Substring(
                    patientId.LastIndexOf("@", StringComparison.Ordinal) + 1);
                var gatewayDiscoveryRepresentation = new GatewayDiscoveryRepresentation(
                    response?.Patient,
                    Guid.NewGuid(),
                    DateTime.Now.ToUniversalTime(),
                    request.TransactionId, //TODO: should be reading transactionId from contract
                    error?.Error,
                    new Resp(request.RequestId));
                logger.LogInformation(LogEvents.Discovery,
                    "Response about to be send for {RequestId} with {@Patient}",
                    request.RequestId,
                    response?.Patient);
                await gatewayClient.SendDataToGateway(PATH_ON_DISCOVER, gatewayDiscoveryRepresentation, cmSuffix, correlationId);
            }
            catch (Exception exception)
            {
                logger.LogError(LogEvents.Discovery, exception, "Error happened for {RequestId}", request.RequestId);
            }
        }
    }
}