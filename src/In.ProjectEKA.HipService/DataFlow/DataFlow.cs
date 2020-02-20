namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Threading.Tasks;
    using HipLibrary.Patient.Model;
    using HipService.Consent;
    using Logger;
    using MessagingQueue;
    using Microsoft.Extensions.Options;

    public class DataFlow : IDataFlow
    {
        private readonly IMessagingQueueManager messagingQueueManager;
        private readonly IDataFlowRepository dataFlowRepository;
        private readonly IConsentRepository consentRepository;
        private readonly ILinkDataRepository linkDataRepository;
        private readonly IOptions<DataFlowConfiguration> dataFlowConfiguration;

        public DataFlow(IDataFlowRepository dataFlowRepository,
            IMessagingQueueManager messagingQueueManager,
            IConsentRepository consentRepository,
            ILinkDataRepository linkDataRepository,
            IOptions<DataFlowConfiguration> dataFlowConfiguration)
        {
            this.dataFlowRepository = dataFlowRepository;
            this.messagingQueueManager = messagingQueueManager;
            this.consentRepository = consentRepository;
            this.linkDataRepository = linkDataRepository;
            this.dataFlowConfiguration = dataFlowConfiguration;
        }

        public async Task<Tuple<HealthInformationResponse, ErrorRepresentation>> HealthInformationRequestFor(
            HealthInformationRequest request)
        {
            var consent = await consentRepository.GetFor(request.Consent.Id);
            if (consent == null) return ConsentArtefactNotFound();

            var dataRequest = new DataRequest(consent.ConsentArtefact.CareContexts, request.HiDataRange,
                request.CallBackUrl, consent.ConsentArtefact.HiTypes, request.TransactionId);
            var result = await dataFlowRepository.SaveRequest(request.TransactionId, request)
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

        private static Tuple<HealthInformationResponse, ErrorRepresentation> ConsentArtefactNotFound()
        {
            return new Tuple<HealthInformationResponse, ErrorRepresentation>(null,
                new ErrorRepresentation(new Error(ErrorCode.ContextArtefactIdNotFound,
                    ErrorMessage.ContextArtefactIdNotFound)));
        }

        public async Task<Tuple<LinkDataResponse, ErrorRepresentation>> HealthInformationFor(
            string linkId,
            string token,
            string transactionId)
        {
            var linkData = await linkDataRepository.GetAsync(linkId);
            if (linkData == null) return LinkDataNotFound();
            if (linkData.Token != token) return InvalidToken();
            if (IsLinkExpired(linkData.DateCreated)) return LinkExpired();

            var linkDataResponse = new LinkDataResponse(transactionId, linkData.Data);
            return new Tuple<LinkDataResponse, ErrorRepresentation>(linkDataResponse, null);
        }

        private bool IsLinkExpired(DateTime dateCreated)
        {
            var linkExpirationTTL = dataFlowConfiguration.Value.DataLinkTTLInMinutes;
            var linkExpirationDateTime = dateCreated.AddMinutes(linkExpirationTTL);
            return linkExpirationDateTime < DateTime.Now;
        }

        private static Tuple<LinkDataResponse, ErrorRepresentation> LinkDataNotFound()
        {
            return ErrorResponse(new Error(ErrorCode.LinkDataNotFound, ErrorMessage.LinkDataNotFound));
        }

        private static Tuple<LinkDataResponse, ErrorRepresentation> LinkExpired()
        {
            return ErrorResponse(new Error(ErrorCode.LinkExpired, ErrorMessage.LinkExpired));
        }

        private static Tuple<LinkDataResponse, ErrorRepresentation> InvalidToken()
        {
            return ErrorResponse(new Error(ErrorCode.InvalidToken, ErrorMessage.InvalidToken));
        }

        private static Tuple<LinkDataResponse, ErrorRepresentation> ErrorResponse(Error error)
        {
            var errorResponse = new ErrorRepresentation(error);
            return new Tuple<LinkDataResponse, ErrorRepresentation>(null, errorResponse);
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