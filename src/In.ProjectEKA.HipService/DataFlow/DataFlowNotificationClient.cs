namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Common;
    using Gateway;
    using Gateway.Model;
    using Logger;
    using Model;
    using static Common.HttpRequestHelper;
    using HipService.Consent;

    public class DataFlowNotificationClient
    {
        private readonly HttpClient httpClient;
        private readonly CentralRegistryClient centralRegistryClient;
        private readonly GatewayClient gatewayClient;
        private readonly IConsentRepository consentRepository;
        private static readonly string HealthInformationNotificationPath = "/health-information/notification";


        public DataFlowNotificationClient(HttpClient httpClient,
                                          CentralRegistryClient centralRegistryClient,
                                          GatewayClient gatewayClient,
                                          IConsentRepository consentRepository)
        {
            this.httpClient = httpClient;
            this.centralRegistryClient = centralRegistryClient;
            this.gatewayClient = gatewayClient;
            this.consentRepository = consentRepository;
        }

        public virtual async Task NotifyGateway( DataNotificationRequest dataNotificationRequest)
        {
            var consent = await consentRepository.GetFor(dataNotificationRequest.ConsentId);
            var notificationRequest = new GatewayDataNotificationRequest(Guid.NewGuid(),
                                                                         DateTime.Now.ToUniversalTime(),
                                                                         new DataFlowNotificationRequest(
                                                                             dataNotificationRequest.TransactionId,
                                                                             dataNotificationRequest.ConsentId,
                                                                             dataNotificationRequest.DoneAt,
                                                                             dataNotificationRequest.Notifier,
                                                                             dataNotificationRequest.StatusNotification
                                                                             )
                                                                         );
            await gatewayClient.SendDataToGateway(GatewayPathConstants.HealthInformationNotifyGatewayPath,
                                                  notificationRequest,
                                                  consent.ConsentArtefact.ConsentManager.Id);
        }

        [Obsolete]
        public virtual async Task NotifyCm(string url, DataNotificationRequest dataNotificationRequest)
        {
            await PostTo(url, dataNotificationRequest);
        }

        [Obsolete]
        private async Task PostTo(string url, DataNotificationRequest dataNotificationRequest)
        {
            try
            {
                var token = await centralRegistryClient.Authenticate();
                token.MatchSome(async accessToken =>
                {
                    try
                    {
                        await httpClient
                            .SendAsync(CreateHttpRequest(url + HealthInformationNotificationPath,
                                dataNotificationRequest,
                                accessToken))
                            .ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Log.Fatal(ex, "Error happened");
                    }
                });
                token.MatchNone(() => Log.Information("Data transfer notification to CM failed"));
            }
            catch (Exception exception)
            {
                Log.Error(exception, exception.StackTrace);
            }
        }
    }
}