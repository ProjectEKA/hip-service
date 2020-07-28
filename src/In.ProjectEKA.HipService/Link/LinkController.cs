// ReSharper disable MemberCanBePrivate.Global
namespace In.ProjectEKA.HipService.Link
{
    using System;
    using System.Threading.Tasks;
    using DefaultHip.Patient;
    using Discovery;
    using Gateway;
    using Gateway.Model;
    using Hangfire;
    using HipLibrary.Patient.Model;
    using Logger;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
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
            var discoveryRequest = await discoveryRequestRepository.GetRequestFor(request.TransactionId);
            request.Patient.ReferenceNumber = discoveryRequest.PatientReferenceNumber;
            var cmUserId = request.Patient.Id;
            var cmSuffix = cmUserId.Substring(
                cmUserId.LastIndexOf("@", StringComparison.Ordinal) + 1);
            var patient = new LinkEnquiry(
                cmSuffix,
                cmUserId,
                request.Patient.ReferenceNumber,
                request.Patient.CareContexts);
            patient = GetUnmaskedLinkEnquiry(patient);
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
                linkedPatientRepresentation.Meta.CommunicationHint = 
                    new MaskingUtility().MaskMobileNumber(linkedPatientRepresentation.Meta.CommunicationHint);
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
                var (patientLinkResponse, cmId, error) = await linkPatient
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
        private LinkEnquiry GetUnmaskedLinkEnquiry(LinkEnquiry linkEnquiry)
        {
            var maskingUtility = new MaskingUtility();
            foreach (var careContextEnquiry in linkEnquiry.CareContexts)
            {
                careContextEnquiry.ReferenceNumber = maskingUtility.UnmaskData(careContextEnquiry.ReferenceNumber);
            }
            return linkEnquiry;
        }
        
           
    }
}