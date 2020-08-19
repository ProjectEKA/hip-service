namespace In.ProjectEKA.HipService.Gateway.Model
{
    using System;
    using HipLibrary.Patient.Model;

    public class GatewayLinkResponse
    {
        public GatewayLinkResponse(LinkEnquiryRepresentation link,
            Error error, Resp resp, string transactionId, DateTime timestamp, Guid requestId)
        {
            Link = link;
            Error = error;
            Resp = resp;
            TransactionId = transactionId;
            Timestamp = timestamp;
            RequestId = requestId;
        }

        public LinkEnquiryRepresentation Link { get; }

        public Guid RequestId { get; }

        public DateTime Timestamp { get; }

        public string TransactionId { get; }

        public Error Error { get; }

        public Resp Resp { get; }
    }
}