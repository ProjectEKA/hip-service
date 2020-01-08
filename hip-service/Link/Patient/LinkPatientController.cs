using System;
using System.Threading.Tasks;
using hip_service.Link.Patient.Dto;
using HipLibrary.Patient;
using HipLibrary.Patient.Models.Request;
using Microsoft.AspNetCore.Mvc;
using HipLibrary.Patient.Models.Request;
using PatientLinkReferenceRequest = hip_service.Link.Patient.Dto.PatientLinkReferenceRequest;

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
            [FromBody]PatientLinkReferenceRequest request)
        {
            var patient = new HipLibrary.Patient.Models.Request.Link(consentManagerId
                , request.Patient.ConsentManagerUserId, request.Patient.ReferenceNumber, request.Patient.CareContexts);
            
            var patientReferenceRequest = new HipLibrary.Patient.Models.Request.PatientLinkReferenceRequest(request.TransactionId,patient);
            var (linkReferenceResponse, error) = await linkPatient.LinkPatients(patientReferenceRequest);

            if (error != null)
            {
                return NotFound(error);
            }

            return Ok(linkReferenceResponse);
        }

        [HttpPost("{linkReferenceNumber}")]
        public async Task<ActionResult> LinkPatient([FromRoute] string linkReferenceNumber, [FromBody]PatientLinkRequest patientLinkRequest)
        {
            var (patientLinkResponse, error) = await linkPatient
                .VerifyAndLinkCareContext(new HipLibrary.Patient.Models.Request.PatientLinkRequest(patientLinkRequest.Token, linkReferenceNumber));

            if (error != null)
            {
                return NotFound(error);
            }

            return Ok(patientLinkResponse);
        }
    }
}