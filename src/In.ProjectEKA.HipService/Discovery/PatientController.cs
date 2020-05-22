using System;
using System.Threading.Tasks;
using Hangfire;
using In.ProjectEKA.HipLibrary.Patient.Model;
using In.ProjectEKA.HipService.Common;
using In.ProjectEKA.HipService.Gateway;
using In.ProjectEKA.HipService.Gateway.Model;
using In.ProjectEKA.HipService.Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static In.ProjectEKA.HipService.Gateway.GatewayPathConstants;

namespace In.ProjectEKA.HipService.Discovery
{
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

    [Route("care-contexts/discover")]
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

        [HttpPost]
        public AcceptedResult DiscoverPatientCareContexts(DiscoveryRequest request)
        {
            backgroundJob.Enqueue(() => GetPatientCareContext(request));
            return Accepted();
        }

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