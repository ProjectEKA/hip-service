using hip_library.Patient;
using hip_library.Patient.models.dto;
using Microsoft.AspNetCore.Mvc;

namespace hip_service.Discovery.Patients
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly IDiscovery patientDiscovery;

        public PatientsController(IDiscovery patientDiscovery)
        {
            this.patientDiscovery = patientDiscovery;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<PatientsResponse> Get(PatientRequest request)
        {
            var patientsResponse = new PatientsResponse(patientDiscovery.GetPatients(request));
            return Ok(patientsResponse);
        }
    }
}