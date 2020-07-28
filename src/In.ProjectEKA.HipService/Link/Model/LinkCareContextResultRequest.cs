using System;
using In.ProjectEKA.HipLibrary.Patient.Model;

namespace In.ProjectEKA.HipService.Link.Model
{
    public class LinkCareContextResultRequest
    {
        public string RequestId { get; }
        public DateTime Timestamp { get; }
        public Acknowledgement Acknowledgement { get; }
        public Error Error { get; }
        public Resp Resp { get; }
        
        public LinkCareContextResultRequest(string requestId, DateTime timestamp, Acknowledgement acknowledgement, Error error, Resp resp)
        {
            RequestId = requestId;
            Timestamp = timestamp;
            Acknowledgement = acknowledgement;
            Error = error;
            Resp = resp;
        }
    }
}


        