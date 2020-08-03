// ReSharper disable MemberCanBePrivate.Global
namespace In.ProjectEKA.HipService.Link
{
    using System;
    using System.Threading.Tasks;
    using Discovery;
    using Gateway;
    using Gateway.Model;
    using Hangfire;
    using HipLibrary.Patient.Model;
    using In.ProjectEKA.HipService.Link.Model;
    using Logger;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using static Common.Constants;

    [Authorize]
    [ApiController]
    public class LinkController : ControllerBase
    {
        private readonly IDiscoveryRequestRepository discoveryRequestRepository;
        private readonly IBackgroundJobClient backgroundJob;
        private readonly LinkPatient linkPatient;
        private readonly GatewayClient gatewayClient;

        public LinkController(
            IDiscoveryRequestRepository discoveryRequestRepository,
            IBackgroundJobClient backgroundJob,
            LinkPatient linkPatient, GatewayClient gatewayClient)
        {
            this.discoveryRequestRepository = discoveryRequestRepository;
            this.backgroundJob = backgroundJob;
            this.linkPatient = linkPatient;
            this.gatewayClient = gatewayClient;
        }

        [HttpPost(PATH_LINKS_LINK_INIT)]
        public AcceptedResult LinkFor(LinkReferenceRequest request)
        {
            backgroundJob.Enqueue(() => LinkPatient(request));
            return Accepted();
        }

        [HttpPost(PATH_LINKS_LINK_CONFIRM)]
        public AcceptedResult LinkPatientFor(LinkPatientRequest request)
        {
            backgroundJob.Enqueue(() => LinkPatientCareContextFor(request));
            return Accepted();
        }

        [NonAction]
        public async Task LinkPatient(LinkReferenceRequest request)
        {
            var cmUserId = request.Patient.Id;
            var cmSuffix = cmUserId.Substring(
                cmUserId.LastIndexOf("@", StringComparison.Ordinal) + 1);
            var patient = new LinkEnquiry(
                cmSuffix,
                cmUserId,
                request.Patient.ReferenceNumber,
                request.Patient.CareContexts);
            try
            {
                var doesRequestExists = await discoveryRequestRepository.RequestExistsFor(
                    request.TransactionId,
                    request.Patient?.Id,
                    request.Patient?.ReferenceNumber);

                ErrorRepresentation errorRepresentation = null;
                if (!doesRequestExists)
                {
                    errorRepresentation = new ErrorRepresentation(
                        new Error(ErrorCode.DiscoveryRequestNotFound, ErrorMessage.DiscoveryRequestNotFound));
                }

                var patientReferenceRequest =
                    new PatientLinkEnquiry(request.TransactionId, request.RequestId, patient);
                var patientLinkEnquiryRepresentation = new PatientLinkEnquiryRepresentation();

                var (linkReferenceResponse, error) = errorRepresentation != null
                    ? (patientLinkEnquiryRepresentation, errorRepresentation)
                    : await linkPatient.LinkPatients(patientReferenceRequest);
                var linkedPatientRepresentation = new LinkEnquiryRepresentation();
                if (linkReferenceResponse != null)
                {
                    linkedPatientRepresentation = linkReferenceResponse.Link;
                }
                var response = new GatewayLinkResponse(
                    linkedPatientRepresentation,
                    error?.Error,
                    new Resp(request.RequestId),
                    request.TransactionId,
                    DateTime.Now.ToUniversalTime(),
                    Guid.NewGuid());

                await gatewayClient.SendDataToGateway(PATH_ON_LINK_INIT, response, cmSuffix);
            }
            catch (Exception exception)
            {
                Log.Error(exception, exception.StackTrace);
            }
        }

        [NonAction]
        public async Task LinkPatientCareContextFor(LinkPatientRequest request)
        {
            try
            {
                var (patientLinkResponse,cmId, error) = await linkPatient
                    .VerifyAndLinkCareContext(new LinkConfirmationRequest(request.Confirmation.Token,
                        request.Confirmation.LinkRefNumber));
                LinkConfirmationRepresentation linkedPatientRepresentation = null;
                if (patientLinkResponse != null)
                {
                    linkedPatientRepresentation = patientLinkResponse.Patient;
                }

                var response = new GatewayLinkConfirmResponse(
                    Guid.NewGuid(),
                    DateTime.Now.ToUniversalTime(),
                    linkedPatientRepresentation,
                    error?.Error,
                    new Resp(request.RequestId));
                await gatewayClient.SendDataToGateway(PATH_ON_LINK_CONFIRM, response, cmId);
            }
            catch (Exception exception)
            {
                Log.Error(exception, exception.StackTrace);
            }
        }

        [HttpPost(PATH_ON_AUTH_INIT)]
        public AcceptedResult OnAuthInit(AuthOnInitRequest request)
        {
            Log.Information("Auth on init request received." +
                            $" RequestId:{request.RequestId}, " +
                            $" Timestamp:{request.Timestamp},");
            if (request.Error != null)
            {
                Log.Information($" Error Code:{request.Error.Code}," +
                                $" Error Message:{request.Error.Message},");
            }
            else if (request.AuthInit != null)
            {
                Log.Information($" Transaction Id:{request.AuthInit.TransactionId},");
                Log.Information($" Auth Type:{request.AuthInit.AuthType},");
                Log.Information($" Auth Meta Mode:{request.AuthInit.Meta.Mode},");
                Log.Information($" Auth Meta Hint:{request.AuthInit.Meta.Hint},");
                Log.Information($" Auth Meta Expiry:{request.AuthInit.Meta.Expiry},");
            }
            Log.Information($" Resp RequestId:{request.Resp.RequestId}");
            return Accepted();
        }

        [HttpPost(PATH_ON_ADD_CONTEXTS)]
        public AcceptedResult HipLinkOnAddContexts(HipLinkContextConfirmation confirmation) {
            Log.Information("Link on-add-context received." +
                            $" RequestId:{confirmation.RequestId}, " +
                            $" Timestamp:{confirmation.Timestamp}");
            if (confirmation.Error != null)
            {
                Log.Information($" Error Code:{confirmation.Error.Code}," +
                                $" Error Message:{confirmation.Error.Message}");
            }
            else if (confirmation.Acknowledgement != null)
            {
                Log.Information($" Acknowledgment Status:{confirmation.Acknowledgement.Status}");
            }
            Log.Information($" Resp RequestId:{confirmation.Resp.RequestId}");
            return Accepted();
        }
    }
}