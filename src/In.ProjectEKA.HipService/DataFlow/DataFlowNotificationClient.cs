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

    public class DataFlowNotificationClient
    {
        private readonly HttpClient httpClient;
        private readonly CentralRegistryClient centralRegistryClient;
        private readonly GatewayClient gatewayClient;
        private static readonly string HealthInformationNotificationPath = "/health-information/notification";


        public DataFlowNotificationClient(HttpClient httpClient,
                                          CentralRegistryClient centralRegistryClient,
                                          GatewayClient gatewayClient
                                          )
        {
            this.httpClient = httpClient;
            this.centralRegistryClient = centralRegistryClient;
            this.gatewayClient = gatewayClient;
        }

        public virtual async Task NotifyGateway(string cmSuffix, DataNotificationRequest dataNotificationRequest)
        {
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
                                                  cmSuffix);
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
                token.MatchNone(() => Log.Information("Data transfer notification to Gateway failed"));
            }
            catch (Exception exception)
            {
                Log.Error(exception, exception.StackTrace);
            }
        }
    }
}