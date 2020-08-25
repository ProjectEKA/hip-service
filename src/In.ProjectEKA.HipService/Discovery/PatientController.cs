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

        /// <summary>
        /// Discover patient's accounts
        /// </summary>
        /// <remarks>
        /// Return only one patient record with (potentially masked) associated care contexts
        /// 1. At least one of the verified identifier matches.
        /// 2. Filter out records using unverified, firstName, secondName, gender and dob
        /// if there are more than one patient records found from step 1.
        /// 3. Store the discover request entry with transactionId and care contexts discovered for a given request
        /// </remarks>
        /// <response code="202">Request accepted</response>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public ActionResult DiscoverPatientCareContexts([FromBody, BindRequired] DiscoveryRequest request)
        {
            logger.LogInformation(LogEvents.Discovery, "discovery request received for {Id} with {RequestId}",
                request.Patient.Id, request.RequestId);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            backgroundJob.Enqueue(() => GetPatientCareContext(request));
            return Accepted();
        }

        [NonAction]
        public async Task GetPatientCareContext(DiscoveryRequest request)
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
                await gatewayClient.SendDataToGateway(PATH_ON_DISCOVER, gatewayDiscoveryRepresentation, cmSuffix);
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
                await gatewayClient.SendDataToGateway(PATH_ON_DISCOVER, gatewayDiscoveryRepresentation, cmSuffix);
                logger.LogError(LogEvents.Discovery, exception, "Error happened for {RequestId}", request.RequestId);
            }
        }
    }
}
