using System;
using System.Threading.Tasks;
using HipLibrary.Patient;
using HipLibrary.Patient.Model.Request;
using Microsoft.AspNetCore.Mvc;

namespace hip_service.Discovery.Patient
{
    [Route("patients/discover/")]
    [ApiController]
    public class PatientsController : Controller
    {
        private readonly IDiscovery patientDiscovery;

        public PatientsController(IDiscovery patientDiscovery)
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