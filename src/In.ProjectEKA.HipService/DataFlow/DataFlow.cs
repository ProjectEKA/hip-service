namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using HipLibrary.Patient.Model;
    using HipService.Consent;
    using Logger;
    using MessagingQueue;
    using Microsoft.Extensions.Options;
    using Model;

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
                request.CallBackUrl, consent.ConsentArtefact.HiTypes, request.TransactionId, request.KeyMaterial);
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
                if (IsExpired(request.KeyMaterial.DhPublicKey.Expiry))
                {
                    var errorResponse = new ErrorRepresentation(new Error(ErrorCode.ExpiredKeyPair,
                        ErrorMessage.ExpiredKeyPair));
                    return new Tuple<HealthInformationTransactionResponse, ErrorRepresentation>(null, errorResponse);
                }
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
            return healthInformation
                .Map(information => HealthInformation(token, transactionId, information))
                .ValueOr(ErrorOf(ErrorResponse.HealthInformationNotFound));
        }

        private Tuple<HealthInformationResponse, ErrorRepresentation> HealthInformation(
            string token,
            string transactionId,
            HealthInformation information)
        {
            if (information.Token != token) return ErrorOf(ErrorResponse.InvalidToken);
            if (IsLinkExpired(information.DateCreated)) return ErrorOf(ErrorResponse.LinkExpired);

            return new Tuple<HealthInformationResponse, ErrorRepresentation>(
                new HealthInformationResponse(transactionId, information.Data), null);
        }

        private bool IsLinkExpired(DateTime dateCreated)
        {
            var linkExpirationTTL = dataFlowConfiguration.Value.DataLinkTTLInMinutes;
            var linkExpirationDateTime = dateCreated.AddMinutes(linkExpirationTTL);
            return linkExpirationDateTime < DateTime.Now;
        }

        private static Tuple<HealthInformationResponse, ErrorRepresentation> ErrorOf(Error error)
        {
            var errorResponse = new ErrorRepresentation(error);
            return new Tuple<HealthInformationResponse, ErrorRepresentation>(null, errorResponse);
        }

        private static bool IsExpired(string expiryDate)
        {
            var expiryDateTime =
                DateTime.ParseExact(expiryDate, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture);
            var currentDate = DateTime.Today;
            return expiryDateTime < currentDate;
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