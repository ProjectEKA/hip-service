using System.Threading.Tasks;
using hip_library.Patient.models;
using Microsoft.AspNetCore.Mvc;

namespace hip_service.Discovery.Patient
{
    using System;

    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : Controller
    {
        private readonly Patients.PatientsDiscovery patientDiscovery;

        public PatientsController(Patients.PatientsDiscovery patientDiscovery)
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