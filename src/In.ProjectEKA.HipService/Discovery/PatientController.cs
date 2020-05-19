using System;
using Hangfire;
using In.ProjectEKA.HipService.Link;

namespace In.ProjectEKA.HipService.Discovery
{
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

        public CareContextDiscoveryController(PatientDiscovery patientDiscovery)
        {
            this.patientDiscovery = patientDiscovery;
        }

        [HttpPost]
        public AcceptedResult DiscoverPatientCareContexts(DiscoveryRequest request)
        {
            BackgroundJob.Enqueue(() => GetPatientCareContext(request));
            return Accepted();
        }

        public Task GetPatientCareContext(DiscoveryRequest request)
        {
            var (discoveryResponse, errorRepresentation) =
                patientDiscovery.PatientFor(request).GetAwaiter().GetResult();
            var timestamp = DateTime.Now.ToUniversalTime().ToString(Constants.DateTimeFormat);
            GatewayDiscoveryRepresentation response = new GatewayDiscoveryRepresentation(
                discoveryResponse.Patient,
                new Guid(),
                timestamp,
                request.TransactionId, //TODO: should be reading transactionId from contract
                errorRepresentation.Error, new Resp(request.RequestId));

            return Task.CompletedTask;
        }
    }
}