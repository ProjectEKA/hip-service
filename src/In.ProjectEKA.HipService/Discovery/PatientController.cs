using In.ProjectEKA.HipService.Common;

namespace In.ProjectEKA.HipService.Discovery
{
    using System;
    using Hangfire;
    using System.Threading.Tasks;
    using HipLibrary.Patient.Model;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    [Route("patients/discover/carecontexts")]
    [ApiController]
    public class DiscoveryController : Controller
    {
        private readonly PatientDiscovery patientDiscovery;

        public DiscoveryController(PatientDiscovery patientDiscovery)
        {
            this.patientDiscovery = patientDiscovery;
        }

        [HttpPost]
        public async Task<ActionResult> Discover(DiscoveryRequest request)
        {
            var (discoveryResponse, error) = await patientDiscovery.PatientFor(request);

            if (error != null) return NotFound(error);

            return Ok(discoveryResponse);
        }
    }

    [Route("patients/care-contexts/discover")]
    [ApiController]
    public class CareContextDiscoveryController : Controller
    {
        private readonly PatientDiscovery patientDiscovery;
        private readonly GatewayClient gatewayClient;

        public CareContextDiscoveryController(PatientDiscovery patientDiscovery, GatewayClient gatewayClient)
        {
            this.patientDiscovery = patientDiscovery;
            this.gatewayClient = gatewayClient;
        }

        [HttpPost]
        public AcceptedResult DiscoverPatientCareContexts(DiscoveryRequest request)
        {
            // should use cancellation token
            // https://docs.hangfire.io/en/latest/background-methods/using-cancellation-tokens.html
            BackgroundJob.Enqueue(() => GetPatientCareContext(request));
            return Accepted();
        }

        public async Task<int> GetPatientCareContext(DiscoveryRequest request)
        {
            var (discoveryResponse, errorRepresentation) = await patientDiscovery.PatientFor(request);
            var patientId = request.Patient.Id;
            var cmSuffix = patientId.Substring(patientId.LastIndexOf("@", StringComparison.Ordinal) + 1);

            var gatewayDiscoveryRepresentation = new GatewayDiscoveryRepresentation(
                discoveryResponse?.Patient,
                Guid.NewGuid(),
                DateTime.Now.ToUniversalTime(),
                request.TransactionId, //TODO: should be reading transactionId from contract
                errorRepresentation?.Error,
                new Resp(request.RequestId));
            await gatewayClient.SendDataToGateway(gatewayDiscoveryRepresentation, cmSuffix);
            return 0;
        }
    }
}