using System;
using System.Threading.Tasks;
using Hangfire;
using In.ProjectEKA.HipLibrary.Patient.Model;
using In.ProjectEKA.HipService.Discovery;
using In.ProjectEKA.HipService.Gateway;
using In.ProjectEKA.HipService.Gateway.Model;
using In.ProjectEKA.HipService.Logger;
using Microsoft.AspNetCore.Mvc;

namespace In.ProjectEKA.HipService.Link
{
    [ApiController]
    [Route("links/link/init")]
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

        [HttpPost]
        public AcceptedResult LinkFor(PatientLinkReferenceRequest request)
        {
            backgroundJob.Enqueue(() => LinkPatient(request));
            return Accepted();
        }

        public async Task LinkPatient(PatientLinkReferenceRequest request)
        {
            var cmUserId = request.Patient.ConsentManagerUserId;
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
                    request.Patient?.ConsentManagerUserId,
                    request.Patient?.ReferenceNumber);

                ErrorRepresentation errorRepresentation = null;
                if (!doesRequestExists)
                {
                    errorRepresentation = new ErrorRepresentation(
                        new Error(ErrorCode.DiscoveryRequestNotFound, ErrorMessage.DiscoveryRequestNotFound));
                }

                var patientReferenceRequest =
                    new PatientLinkEnquiry(request.TransactionId, request.RequestId, patient);
                var (linkReferenceResponse, error) = errorRepresentation != null
                    ? (null, errorRepresentation)
                    : await linkPatient.LinkPatients(patientReferenceRequest);
                var response = new GatewayLinkResponse(linkReferenceResponse, error);
                await gatewayClient.SendDataToGateway(GatewayPathConstants.OnLinkInitPath, response, cmSuffix);
            }
            catch (Exception exception)
            {
                Log.Error(exception, exception.StackTrace);
            }
        }
    }
}