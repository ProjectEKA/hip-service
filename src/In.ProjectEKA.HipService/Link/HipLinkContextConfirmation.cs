using System;
using In.ProjectEKA.HipLibrary.Patient.Model;
using In.ProjectEKA.HipService.Link.Model;

namespace In.ProjectEKA.HipService.Link {
    public class HipLinkContextConfirmation
    {
        public string RequestId { get; }
        public DateTime Timestamp { get; }
        public Error Error { get; }
        public Resp Resp { get; }
        public AddContextsAcknowledgement Acknowledgement { get; }

        public HipLinkContextConfirmation(string requestId, DateTime timestamp, AddContextsAcknowledgement acknowledgement, Error error, Resp resp)
        {
            RequestId = requestId;
            Timestamp = timestamp;
            Acknowledgement = acknowledgement;
            Error = error;
            Resp = resp;
        }
    }
}
    