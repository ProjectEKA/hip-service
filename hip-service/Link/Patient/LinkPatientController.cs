using System;
using System.Diagnostics;
using System.Threading.Tasks;
using HipLibrary.Patient;
using HipLibrary.Patient.Model.Response;
using Microsoft.AspNetCore.Mvc;
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
            var patient = new HipLibrary.Patient.Model.Request.Link(consentManagerId
                , request.Patient.ConsentManagerUserId, request.Patient.ReferenceNumber, request.Patient.CareContexts);
            
            var patientReferenceRequest = new HipLibrary.Patient.Model.Request.PatientLinkReferenceRequest(request.TransactionId,patient);
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
                .VerifyAndLinkCareContext(new HipLibrary.Patient.Model.Request.PatientLinkRequest(patientLinkRequest.Token, linkReferenceNumber));

            return error != null ? ReturnServerResponse(error) : Ok(patientLinkResponse);
        }
        
        private ActionResult ReturnServerResponse(ErrorResponse errorResponse)
        {
            return errorResponse.Error.Code switch
            {
                ErrorCode.OtpExpired => (ActionResult) BadRequest(errorResponse),
                ErrorCode.MultiplePatientsFound => NotFound(errorResponse),
                ErrorCode.NoPatientFound => NotFound(errorResponse),
                ErrorCode.OtpGenerationFailed => BadRequest(errorResponse),
                ErrorCode.OtpInValid => NotFound(errorResponse),
                ErrorCode.ServerInternalError => BadRequest(errorResponse),
                ErrorCode.CareContextNotFound => NotFound(errorResponse),
                ErrorCode.NoLinkRequestFound => NotFound(errorResponse),
                _ => NotFound(errorResponse)
            };
        }
    }
}