using PatientLinkRequestLib = In.ProjectEKA.HipLibrary.Patient.Model.Request.PatientLinkRequest;
using PatientLinkRefRequest = In.ProjectEKA.HipLibrary.Patient.Model.Request.PatientLinkReferenceRequest;
using LinkLib = In.ProjectEKA.HipLibrary.Patient.Model.Request.Link;
namespace In.ProjectEKA.HipService.Link
{
    using System;
    using System.Threading.Tasks;
    using Discovery;
    using HipLibrary.Patient;
    using HipLibrary.Patient.Model.Response;
    using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult> LinkPatientCareContexts(
            [FromHeader(Name = "X-ConsentManagerID")] string consentManagerId,
            [FromBody] PatientLinkReferenceRequest request)
        {
            var patient = new LinkLib(
                consentManagerId,
                request.Patient.ConsentManagerUserId,
                request.Patient.ReferenceNumber,
                request.Patient.CareContexts);
            var doesRequestExists = await discoveryRequestRepository.RequestExistsFor(request.TransactionId);
            if (!doesRequestExists)
            {
                return ReturnServerResponse(new ErrorResponse(new Error(ErrorCode.ServerInternalError,
                    ErrorMessage.TransactionIdNotFound)));
            }
            var patientReferenceRequest =
                new PatientLinkRefRequest(request.TransactionId, patient);
            var (linkReferenceResponse, error) = await linkPatient.LinkPatients(patientReferenceRequest);
            return error != null ? ReturnServerResponse(error) : Ok(linkReferenceResponse);
        }

        [HttpPost("{linkReferenceNumber}")]
        public async Task<ActionResult> LinkPatient(
            [FromRoute] string linkReferenceNumber,
            [FromBody] PatientLinkRequest patientLinkRequest)
        {
            var (patientLinkResponse, error) = await linkPatient
                .VerifyAndLinkCareContext(new PatientLinkRequestLib(patientLinkRequest.Token,
                    linkReferenceNumber));
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