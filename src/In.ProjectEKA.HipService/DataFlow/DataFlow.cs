
namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Threading.Tasks;
    using HipLibrary.Patient.Model;
    using MessagingQueue;

    public class DataFlow : IDataFlow
    {
        private readonly IMessagingQueueManager messagingQueueManager;
        private readonly IDataFlowRepository dataFlowRepository;
        private readonly IDataFlowArtefactRepository dataFlowArtefactRepository;

        public DataFlow(IDataFlowRepository dataFlowRepository, IDataFlowArtefactRepository dataFlowArtefactRepository, IMessagingQueueManager messagingQueueManager)
        {
            this.dataFlowRepository = dataFlowRepository;
            this.dataFlowArtefactRepository = dataFlowArtefactRepository;
            this.messagingQueueManager = messagingQueueManager;
        }
        
        public async Task<Tuple<HealthInformationResponse, ErrorRepresentation>> HealthInformationRequestFor(HealthInformationRequest request)
        {
            var (dataFlowArtefact, error) = await dataFlowArtefactRepository.GetFor(request);
            if (error != null)
            {
                return new Tuple<HealthInformationResponse, ErrorRepresentation>(null, error);
            }
            
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
                PublishArtefact(dataFlowArtefact);
            }
            
            return new Tuple<HealthInformationResponse, ErrorRepresentation>(response, errorRepresentation);
        }

        private void PublishArtefact(DataFlowArtefact artefact)
        {
            messagingQueueManager.Publish(
                artefact,
                MessagingQueueConstants.DataRequestExchangeName,
                MessagingQueueConstants.DataRequestExchangeType,
                MessagingQueueConstants.DataRequestRoutingKey);
        }
    }
}