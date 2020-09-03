namespace In.ProjectEKA.HipService.Discovery
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Gateway;
    using Gateway.Model;
    using static Common.Constants;
    using Hangfire;
    using HipLibrary.Patient.Model;
    using Logger;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.Extensions.Logging;
    using Common;


    [Authorize]
    [Route(PATH_CARE_CONTEXTS_DISCOVER)]
    [ApiController]
    public class CareContextDiscoveryController : Controller
    {
        private const string SuccessMessage = "Patient record with one or more care contexts found";
        private const string ErrorMessage = "No Matching Record Found or More than one Record Found";
        
        private readonly IPatientDiscovery patientDiscovery;
        private readonly IGatewayClient gatewayClient;
        private readonly IBackgroundJobClient backgroundJob;
        private readonly ILogger<CareContextDiscoveryController> logger;

        public CareContextDiscoveryController(IPatientDiscovery patientDiscovery,
            IGatewayClient gatewayClient,
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
            logger.LogInformation(LogEvents.Discovery, "discovery request received for {Id} with {RequestId}",
                request.Patient.Id, request.RequestId);
            backgroundJob.Enqueue(() => GetPatientCareContext(request, correlationId));
            return Accepted();
        }

        [NonAction]
        public async Task GetPatientCareContext(DiscoveryRequest request, string correlationId)
        {
            var patientId = request.Patient.Id;
            var cmSuffix = patientId.Substring(patientId.LastIndexOf("@", StringComparison.Ordinal) + 1);
            try {
                var (response, error) = await patientDiscovery.PatientFor(request);
                var gatewayDiscoveryRepresentation = new GatewayDiscoveryRepresentation(
                    response?.Patient,
                    Guid.NewGuid(),
                    DateTime.Now.ToUniversalTime(),
                    request.TransactionId, //TODO: should be reading transactionId from contract
                    error?.Error,
                    new DiscoveryResponse(request.RequestId, error == null ? HttpStatusCode.OK : HttpStatusCode.NotFound, error == null ? SuccessMessage : ErrorMessage));
                logger.LogInformation(LogEvents.Discovery,
                    "Response about to be send for {RequestId} with {@Patient}",
                    request.RequestId,
                    response?.Patient);
                await gatewayClient.SendDataToGateway(PATH_ON_DISCOVER, gatewayDiscoveryRepresentation, cmSuffix, correlationId);
            }
            catch (Exception exception)
            {
                var gatewayDiscoveryRepresentation = new GatewayDiscoveryRepresentation(
                    null,
                    Guid.NewGuid(),
                    DateTime.Now.ToUniversalTime(),
                    request.TransactionId, //TODO: should be reading transactionId from contract
                    new Error(ErrorCode.ServerInternalError, "Unreachable external service"),
                    new DiscoveryResponse(request.RequestId, HttpStatusCode.InternalServerError, "Unreachable external service"));
                await gatewayClient.SendDataToGateway(PATH_ON_DISCOVER, gatewayDiscoveryRepresentation, cmSuffix,correlationId);
                logger.LogError(LogEvents.Discovery, exception, "Error happened for {RequestId}", request.RequestId);
            }
        }
    }
}
