namespace In.ProjectEKA.HipService.Gateway.Model
{
    using System;
    using HipLibrary.Patient.Model;

    public class GatewayLinkResponse
    {
        public PatientLinkEnquiryRepresentation PatientLinkEnquiryRepresentation { get; }

        public Guid RequestId { get; }

        public DateTime Timestamp { get; }

        public string TransactionId { get; }

        public Error Error { get; }

        public Resp Resp { get; }

        public GatewayLinkResponse(PatientLinkEnquiryRepresentation patientLinkEnquiryRepresentation,
            Error error, Resp resp, string transactionId, DateTime timestamp, Guid requestId)
        {
            PatientLinkEnquiryRepresentation = patientLinkEnquiryRepresentation;
            Error = error;
            Resp = resp;
            TransactionId = transactionId;
            Timestamp = timestamp;
            RequestId = requestId;
        }
    }
}