namespace In.ProjectEKA.HipService.Link
{
    using System;
    using HipLibrary.Patient.Model;
    using Model;

    public class HipLinkContextConfirmation
    {
        public HipLinkContextConfirmation(string requestId, DateTime timestamp,
            AddContextsAcknowledgement acknowledgement, Error error, Resp resp)
        {
            RequestId = requestId;
            Timestamp = timestamp;
            Acknowledgement = acknowledgement;
            Error = error;
            Resp = resp;
        }

        public string RequestId { get; }
        public DateTime Timestamp { get; }
        public Error Error { get; }
        public Resp Resp { get; }
        public AddContextsAcknowledgement Acknowledgement { get; }
    }
}