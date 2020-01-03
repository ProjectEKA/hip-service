using System;
using System.Threading.Tasks;
using health_information_provider_library.Patient.models;
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
        public async Task<ActionResult> LinkPatientCareContexts([FromHeader(Name = "X-ConsentManagerID")]string consentManagerId,
            [FromBody]LinkReference request)
        {
            PatientLinkReferenceRequest patientReferenceRequest = new PatientLinkReferenceRequest(consentManagerId,
                request.ConsentManagerUserId, request.PatientReferenceNumber, request.CareContexts);
            
            var (linkReferenceResponse, error) = await linkPatient.LinkPatients(patientReferenceRequest);

            if (error != null) return NotFound(error);

            return Ok(linkReferenceResponse);
        }

        [HttpPost("{linkReferenceNumber}")]
        public async Task<ActionResult> LinkPatient([FromRoute] string linkReferenceNumber, [FromBody]LinkToken linkToken)
        {
            var (patientLinkResponse, error) = await linkPatient
                .VerifyAndLinkCareContext(new PatientLinkRequest(linkToken.Token, linkReferenceNumber));

            if (error != null) return NotFound(error);

            return Ok(patientLinkResponse);
        }
    }
}