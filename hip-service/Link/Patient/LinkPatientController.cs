using System;
using System.Threading.Tasks;
using hip_library.Patient;
using hip_library.Patient.models.dto;
using Microsoft.AspNetCore.Mvc;

namespace hip_service.Link.Patient
{
    [Route("patients/link")]
    [ApiController]
    public class LinkPatientController: ControllerBase
    {
        private readonly ILink linkPatient;

        public LinkPatientController(ILink linkPatient)
        {
            this.linkPatient = linkPatient;
        }

        [HttpPost]
        public async Task<ActionResult> LinkPatientCareContexts(PatientLinkReferenceRequest request)
        {
            var (linkReferenceResponse, error) = await linkPatient.LinkPatients(request);

            if (error != null) return NotFound(error);

            return Ok(linkReferenceResponse);
        }
    }
}