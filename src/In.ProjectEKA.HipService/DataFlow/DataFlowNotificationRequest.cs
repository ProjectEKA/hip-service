namespace In.ProjectEKA.HipService.DataFlow
{
    using System;

    public class DataFlowNotificationRequest
    {
        public string TransactionId { get; }
        public string ConsentId { get; }
        public DateTime DoneAt { get; }
        public Notifier Notifier { get; }
        public StatusNotification StatusNotification { get; }

        public DataFlowNotificationRequest(string transactionId, string consentId, DateTime doneAt, Notifier notifier, StatusNotification statusNotification)
        {
            TransactionId = transactionId;
            ConsentId = consentId;
            DoneAt = doneAt;
            Notifier = notifier;
            StatusNotification = statusNotification;
        }
    }
}