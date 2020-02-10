namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Threading.Tasks;
    using HipLibrary.Patient.Model;
    using Logger;
    using MessagingQueue;

    public class DataFlow : IDataFlow
    {
        private readonly IMessagingQueueManager messagingQueueManager;
        private readonly IDataFlowRepository dataFlowRepository;

        public DataFlow(IDataFlowRepository dataFlowRepository, IMessagingQueueManager messagingQueueManager)
        {
            this.dataFlowRepository = dataFlowRepository;
            this.messagingQueueManager = messagingQueueManager;
        }
        
        public async Task<Tuple<HealthInformationResponse, ErrorRepresentation>> HealthInformationRequestFor(HealthInformationRequest request)
        {
            var (artefact, error) = dataFlowRepository.GetFor(request.Consent.Id);
            if (error != null)
            {
                return new Tuple<HealthInformationResponse, ErrorRepresentation>(null,
                    new ErrorRepresentation(new Error(ErrorCode.ContextArtefactIdNotFound,
                        ErrorMessage.ContextArtefactIdNotFound)));
            }
            var dataRequest = new DataRequest(artefact.CareContexts, request.HiDataRange,
                request.CallBackUrl, artefact.HiTypes, request.TransactionId);
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
                PublishArtefact(dataRequest);
            }

            return new Tuple<HealthInformationResponse, ErrorRepresentation>(response, errorRepresentation);
        }

        private void PublishArtefact(DataRequest artefact)
        {
            Log.Information("Publishing artefact: {artefact}", artefact);
            messagingQueueManager.Publish(
                artefact,
                MessagingQueueConstants.DataRequestExchangeName,
                MessagingQueueConstants.DataRequestExchangeType,
                MessagingQueueConstants.DataRequestRoutingKey);
        }
    }
}