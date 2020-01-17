using LinkReferenceRequest = HipLibrary.Patient.Model.Request.PatientLinkRequest;

namespace In.ProjectEKA.HipService.Link.Patient
{
    using System;
    using System.Threading.Tasks;
    using HipLibrary.Patient;
    using HipLibrary.Patient.Model.Request;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("patients/link")]
    public class LinkPatientController : ControllerBase
    {
        private readonly ILink linkPatient;

        public LinkPatientController(ILink linkPatient)
        {
            this.linkPatient = linkPatient;
        }

        [HttpPost]
        public async Task<ActionResult> LinkPatientCareContexts(
            [FromHeader(Name = "X-ConsentManagerID")]
            string consentManagerId,
            [FromBody] PatientLinkReferenceRequest request)
        {
            var patient = new Link(
                consentManagerId,
                request.Patient.ConsentManagerUserId,
                request.Patient.ReferenceNumber,
                request.Patient.CareContexts);
            var patientReferenceRequest = new PatientLinkReferenceRequest(request.TransactionId, patient);
            var (linkReferenceResponse, error) = await linkPatient.LinkPatients(patientReferenceRequest);
            if (error != null) return NotFound(error);

            return Ok(linkReferenceResponse);
        }

        [HttpPost("{linkReferenceNumber}")]
        public async Task<ActionResult> LinkPatient(
            [FromRoute] string linkReferenceNumber,
            [FromBody] PatientLinkRequest patientLinkRequest)
        {
            var (patientLinkResponse, error) = await linkPatient.VerifyAndLinkCareContext(
                new LinkReferenceRequest(patientLinkRequest.Token, linkReferenceNumber));
            if (error != null) return NotFound(error);

            return Ok(patientLinkResponse);
        }
    }
}