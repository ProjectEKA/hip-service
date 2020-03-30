namespace In.ProjectEKA.HipService.Link
{
    using System;
    using System.Threading.Tasks;
    using Discovery;
    using HipLibrary.Patient;
    using HipLibrary.Patient.Model;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    [ApiController]
    [Route("patients/link")]
    public class LinkPatientController : ControllerBase
    {
        private readonly ILink linkPatient;
        private readonly IDiscoveryRequestRepository discoveryRequestRepository;

        public LinkPatientController(
            ILink linkPatient,
            IDiscoveryRequestRepository discoveryRequestRepository)
        {
            this.linkPatient = linkPatient;
            this.discoveryRequestRepository = discoveryRequestRepository;
        }

        [HttpPost]
        public async Task<ActionResult> LinkPatientCareContexts([FromHeader(Name = "X-ConsentManagerID")]
            string consentManagerId,
            [FromBody] PatientLinkReferenceRequest request)
        {
            var patient = new LinkEnquiry(
                consentManagerId,
                request.Patient.ConsentManagerUserId,
                request.Patient.ReferenceNumber,
                request.Patient.CareContexts);
            var doesRequestExists = await discoveryRequestRepository.RequestExistsFor(
                request.TransactionId,
                request.Patient?.ConsentManagerUserId,
                request.Patient?.ReferenceNumber);
            if (!doesRequestExists)
            {
                return ResponseFrom(new ErrorRepresentation(
                    new Error(ErrorCode.DiscoveryRequestNotFound, ErrorMessage.DiscoveryRequestNotFound)));
            }

            var patientReferenceRequest =
                new PatientLinkEnquiry(request.TransactionId, patient);
            var (linkReferenceResponse, error) = await linkPatient.LinkPatients(patientReferenceRequest);
            return error != null ? ResponseFrom(error) : Ok(linkReferenceResponse);
        }

        [HttpPost("{linkReferenceNumber}")]
        public async Task<ActionResult> LinkPatient(
            [FromRoute] string linkReferenceNumber,
            [FromBody] PatientLinkRequest patientLinkRequest)
        {
            var (patientLinkResponse, error) = await linkPatient
                .VerifyAndLinkCareContext(new LinkConfirmationRequest(patientLinkRequest.Token,
                    linkReferenceNumber));
            return error != null ? ResponseFrom(error) : Ok(patientLinkResponse);
        }

        private ActionResult ResponseFrom(ErrorRepresentation errorResponse)
        {
            return errorResponse.Error.Code switch
            {
                ErrorCode.OtpExpired => (ActionResult) BadRequest(errorResponse),
                ErrorCode.MultiplePatientsFound => NotFound(errorResponse),
                ErrorCode.NoPatientFound => NotFound(errorResponse),
                ErrorCode.OtpGenerationFailed => StatusCode(StatusCodes.Status500InternalServerError, errorResponse),
                ErrorCode.OtpInValid => NotFound(errorResponse),
                ErrorCode.ServerInternalError => StatusCode(StatusCodes.Status500InternalServerError, errorResponse),
                ErrorCode.CareContextNotFound => NotFound(errorResponse),
                ErrorCode.NoLinkRequestFound => NotFound(errorResponse),
                ErrorCode.DiscoveryRequestNotFound => NotFound(errorResponse),
                _ => NotFound(errorResponse)
            };
        }
    }
}