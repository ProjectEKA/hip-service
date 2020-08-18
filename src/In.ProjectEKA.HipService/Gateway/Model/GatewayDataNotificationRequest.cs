namespace In.ProjectEKA.HipService.Gateway.Model
{
    using System;
    using DataFlow;

    public class GatewayDataNotificationRequest
    {
        public GatewayDataNotificationRequest(Guid requestId, DateTime timestamp,
            DataFlowNotificationRequest notification)
        {
            RequestId = requestId;
            Timestamp = timestamp;
            Notification = notification;
        }

        public Guid RequestId { get; }
        public DateTime Timestamp { get; }
        public DataFlowNotificationRequest Notification { get; }
    }
}