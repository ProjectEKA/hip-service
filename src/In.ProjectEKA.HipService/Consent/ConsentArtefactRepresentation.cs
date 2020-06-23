namespace In.ProjectEKA.HipService.Consent
{
    using System;
    using Model;

    public class ConsentArtefactRepresentation
    {
        public Notification Notification { get; }

        public DateTime Timestamp { get; }

        public string RequestId { get; }

        public ConsentArtefactRepresentation(Notification notification, DateTime timestamp, string requestId)
        {
            Notification = notification;
            Timestamp = timestamp;
            RequestId = requestId;
        }
    }
}