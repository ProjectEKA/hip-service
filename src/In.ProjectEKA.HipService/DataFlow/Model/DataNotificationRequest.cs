namespace In.ProjectEKA.HipService.DataFlow.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class DataNotificationRequest
    {
        public DataNotificationRequest()
        {
        }

        public DataNotificationRequest(
            string transactionId,
            DateTime doneAt,
            Notifier notifier,
            StatusNotification statusNotification,
            string consentId,
            Guid requestId)
        {
            TransactionId = transactionId;
            DoneAt = doneAt;
            Notifier = notifier;
            StatusNotification = statusNotification;
            ConsentId = consentId;
            RequestId = requestId;
        }

        [Key]
        public string TransactionId { get; }

        public Guid RequestId { get; }
        public string ConsentId { get; }
        public DateTime DoneAt { get; }
        public Notifier Notifier { get; }
        public StatusNotification StatusNotification { get; }
    }
}