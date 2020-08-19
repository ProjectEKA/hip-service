namespace In.ProjectEKA.HipService.Consent
{
    using System;
    using Model;

    public class ConsentArtefactRepresentation
    {
        public ConsentArtefactRepresentation(Notification notification, DateTime timestamp, string requestId)
        {
            Notification = notification;
            Timestamp = timestamp;
            RequestId = requestId;
        }

        public Notification Notification { get; }

        public DateTime Timestamp { get; }

        public string RequestId { get; }
    }
}