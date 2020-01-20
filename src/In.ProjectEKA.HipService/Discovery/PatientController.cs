namespace In.ProjectEKA.HipService.Discovery
{
    using System;
    using System.Threading.Tasks;
    using HipLibrary.Patient;
    using HipLibrary.Patient.Model.Request;
    using Microsoft.AspNetCore.Mvc;

    [Route("patients/discover/")]
    [ApiController]
    public class DiscoveryController : Controller
    {
        private readonly IDiscovery patientDiscovery;

        public DiscoveryController(IDiscovery patientDiscovery)
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
}