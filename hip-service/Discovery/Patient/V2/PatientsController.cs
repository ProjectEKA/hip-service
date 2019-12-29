using System.Threading.Tasks;
using hip_library.Patient.models;
using Microsoft.AspNetCore.Mvc;

namespace hip_service.Discovery.Patient
{
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
            var result = await patientDiscovery.PatientFor(request);
            var patient = result.Item1;
            var error = result.Item2;

            if (error != null) return NotFound(error);

            return Ok(patient);
        }
    }
}