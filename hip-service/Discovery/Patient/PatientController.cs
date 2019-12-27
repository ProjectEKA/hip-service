using System;
using System.Threading.Tasks;
using hip_library.Patient;
using hip_library.Patient.models;
using Microsoft.AspNetCore.Mvc;

namespace hip_service.Discovery.Patient
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IDiscovery patientDiscovery;

        public PatientController(IDiscovery patientDiscovery)
        {
            this.patientDiscovery = patientDiscovery;
        }

        [HttpPost]
        public async Task<ActionResult> Discover(DiscoveryRequest request)
        {
            var (patient, error) = await patientDiscovery.PatientFor(request);

            if (error != null) return NotFound(error);

            return Ok(patient);
        }
    }
}