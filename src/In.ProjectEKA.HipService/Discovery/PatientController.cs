using static In.ProjectEKA.HipService.Gateway.GatewayPathConstants;

namespace In.ProjectEKA.HipService.Discovery
{
    using System;
    using System.Threading.Tasks;
    using Gateway;
    using Gateway.Model;
    using Hangfire;
    using HipLibrary.Patient.Model;
    using Logger;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    [Authorize]
    [Route("v1/care-contexts/discover")]
    [ApiController]
    public class CareContextDiscoveryController : Controller
    {
        private readonly PatientDiscovery patientDiscovery;
        private readonly GatewayClient gatewayClient;
        private readonly IBackgroundJobClient backgroundJob;

        public CareContextDiscoveryController(PatientDiscovery patientDiscovery,
            GatewayClient gatewayClient,
            IBackgroundJobClient backgroundJob)
        {
            this.patientDiscovery = patientDiscovery;
            this.gatewayClient = gatewayClient;
            this.backgroundJob = backgroundJob;
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            backgroundJob.Enqueue(() => GetPatientCareContext(request));
            return Accepted();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task GetPatientCareContext(DiscoveryRequest request)
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
                await gatewayClient.SendDataToGateway(OnDiscoverPath, gatewayDiscoveryRepresentation, cmSuffix);
            }
            catch (Exception exception)
            {
                Log.Error(exception, exception.StackTrace);
            }
        }
    }
}