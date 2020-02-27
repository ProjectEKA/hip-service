namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Threading.Tasks;
    using HipLibrary.Patient.Model;
    using HipService.Consent;
    using Logger;
    using MessagingQueue;

    public class DataFlow : IDataFlow
    {
        private readonly IMessagingQueueManager messagingQueueManager;
        private readonly IDataFlowRepository dataFlowRepository;
        private readonly IConsentRepository consentRepository;

        public DataFlow(IDataFlowRepository dataFlowRepository,
            IMessagingQueueManager messagingQueueManager,
            IConsentRepository consentRepository)
        {
            this.dataFlowRepository = dataFlowRepository;
            this.messagingQueueManager = messagingQueueManager;
            this.consentRepository = consentRepository;
        }

        public async Task<Tuple<HealthInformationResponse, ErrorRepresentation>> HealthInformationRequestFor(
            HealthInformationRequest request)
        {
            var consent = await consentRepository.GetFor(request.Consent.Id);
            if (consent == null)
            {
                return new Tuple<HealthInformationResponse, ErrorRepresentation>(null,
                    new ErrorRepresentation(new Error(ErrorCode.ContextArtefactIdNotFound,
                        ErrorMessage.ContextArtefactIdNotFound)));
            }

            var dataRequest = new DataRequest(consent.ConsentArtefact.CareContexts, request.HiDataRange,
                request.CallBackUrl, consent.ConsentArtefact.HiTypes, request.TransactionId);
            var result = await dataFlowRepository.SaveRequestFor(request.TransactionId, request)
                .ConfigureAwait(false);
            var (response, errorRepresentation) = result.Map(r =>
            {
                var errorResponse = new ErrorRepresentation(new Error(ErrorCode.ServerInternalError,
                    ErrorMessage.InternalServerError));
                return new Tuple<HealthInformationResponse, ErrorRepresentation>(null, errorResponse);
            }).ValueOr(new Tuple<HealthInformationResponse,
                ErrorRepresentation>(new HealthInformationResponse(request.TransactionId), null));

            if (errorRepresentation == null)
            {
                PublishDataRequest(dataRequest);
            }

            return new Tuple<HealthInformationResponse, ErrorRepresentation>(response, errorRepresentation);
        }

        private void PublishDataRequest(DataRequest dataRequest)
        {
            Log.Information("Publishing dataRequest: {dataRequest}", dataRequest);
            messagingQueueManager.Publish(
                dataRequest,
                MessagingQueueConstants.DataRequestExchangeName,
                MessagingQueueConstants.DataRequestExchangeType,
                MessagingQueueConstants.DataRequestRoutingKey);
        }
    }
}