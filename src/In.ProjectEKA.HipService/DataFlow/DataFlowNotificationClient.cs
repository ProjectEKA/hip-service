namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Threading.Tasks;
    using Gateway;
    using Gateway.Model;
    using Model;
    using static Common.Constants;

    public class DataFlowNotificationClient
    {
        private readonly GatewayClient gatewayClient;

        public DataFlowNotificationClient(GatewayClient gatewayClient)
        {
            this.gatewayClient = gatewayClient;
        }

        public virtual async Task NotifyGateway(string cmSuffix, DataNotificationRequest dataNotificationRequest,
            string correlationId)
        {
            var notificationRequest = new GatewayDataNotificationRequest(Guid.NewGuid(),
                DateTime.Now.ToUniversalTime(),
                new DataFlowNotificationRequest(
                    dataNotificationRequest.TransactionId,
                    dataNotificationRequest.ConsentId,
                    dataNotificationRequest.DoneAt,
                    dataNotificationRequest.Notifier,
                    dataNotificationRequest.StatusNotification));
            await gatewayClient.SendDataToGateway(PATH_HEALTH_INFORMATION_NOTIFY_GATEWAY,
                notificationRequest,
                cmSuffix, correlationId);
        }
    }
}