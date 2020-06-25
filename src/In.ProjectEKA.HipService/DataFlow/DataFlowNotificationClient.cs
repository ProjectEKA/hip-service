namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Threading.Tasks;
    using Gateway;
    using Gateway.Model;
    using Model;

    public class DataFlowNotificationClient
    {
        private readonly GatewayClient gatewayClient;
        private static readonly string HealthInformationNotificationPath = "/health-information/notification";

        public DataFlowNotificationClient(GatewayClient gatewayClient)
        {
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
                    dataNotificationRequest.StatusNotification)
                );
            await gatewayClient.SendDataToGateway(GatewayPathConstants.HealthInformationNotifyGatewayPath,
                notificationRequest,
                cmSuffix);
        }
    }
}