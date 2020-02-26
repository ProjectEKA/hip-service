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
        private readonly IHealthInformationRepository healthInformationRepository;
        private readonly IOptions<DataFlowConfiguration> dataFlowConfiguration;

        public DataFlow(IDataFlowRepository dataFlowRepository,
            IMessagingQueueManager messagingQueueManager,
            IConsentRepository consentRepository,
            IHealthInformationRepository healthInformationRepository,
            IOptions<DataFlowConfiguration> dataFlowConfiguration)
        {
            this.dataFlowRepository = dataFlowRepository;
            this.messagingQueueManager = messagingQueueManager;
            this.consentRepository = consentRepository;
            this.healthInformationRepository = healthInformationRepository;
            this.dataFlowConfiguration = dataFlowConfiguration;
        }

        public async Task<Tuple<HealthInformationTransactionResponse, ErrorRepresentation>> HealthInformationRequestFor(
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
                return new Tuple<HealthInformationTransactionResponse, ErrorRepresentation>(null, errorResponse);
            }).ValueOr(new Tuple<HealthInformationTransactionResponse,
                ErrorRepresentation>(new HealthInformationTransactionResponse(request.TransactionId), null));

            if (errorRepresentation == null)
            {
                PublishDataRequest(dataRequest);
            }

            return new Tuple<HealthInformationTransactionResponse, ErrorRepresentation>(response, errorRepresentation);
        }

        private static Tuple<HealthInformationTransactionResponse, ErrorRepresentation> ConsentArtefactNotFound()
        {
            return new Tuple<HealthInformationTransactionResponse, ErrorRepresentation>(null,
                new ErrorRepresentation(new Error(ErrorCode.ContextArtefactIdNotFound,
                    ErrorMessage.ContextArtefactIdNotFound)));
        }

        public async Task<Tuple<HealthInformationResponse, ErrorRepresentation>> HealthInformationFor(
            string informationId,
            string token,
            string transactionId)
        {
            var healthInformation = await healthInformationRepository.GetAsync(informationId);
            if (healthInformation == null) return HealthInformationNotFound();
            if (healthInformation.Token != token) return InvalidToken();
            if (IsLinkExpired(healthInformation.DateCreated)) return LinkExpired();

            var healthInformationResponse = new HealthInformationResponse(transactionId, healthInformation.Data);
            return new Tuple<HealthInformationResponse, ErrorRepresentation>(healthInformationResponse, null);
        }

        private bool IsLinkExpired(DateTime dateCreated)
        {
            var linkExpirationTTL = dataFlowConfiguration.Value.DataLinkTTLInMinutes;
            var linkExpirationDateTime = dateCreated.AddMinutes(linkExpirationTTL);
            return linkExpirationDateTime < DateTime.Now;
        }

        private static Tuple<HealthInformationResponse, ErrorRepresentation> HealthInformationNotFound()
        {
            return ErrorResponse(
                new Error(ErrorCode.HealthInformationNotFound, ErrorMessage.HealthInformationNotFound));
        }

        private static Tuple<HealthInformationResponse, ErrorRepresentation> LinkExpired()
        {
            return ErrorResponse(new Error(ErrorCode.LinkExpired, ErrorMessage.LinkExpired));
        }

        private static Tuple<HealthInformationResponse, ErrorRepresentation> InvalidToken()
        {
            return ErrorResponse(new Error(ErrorCode.InvalidToken, ErrorMessage.InvalidToken));
        }

        private static Tuple<HealthInformationResponse, ErrorRepresentation> ErrorResponse(Error error)
        {
            var errorResponse = new ErrorRepresentation(error);
            return new Tuple<HealthInformationResponse, ErrorRepresentation>(null, errorResponse);
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