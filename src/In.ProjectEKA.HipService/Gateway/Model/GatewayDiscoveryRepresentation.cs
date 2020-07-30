namespace In.ProjectEKA.HipService.Gateway.Model
{
    using System;
    using HipLibrary.Patient.Model;

    public class GatewayDiscoveryRepresentation
    {
        public PatientEnquiryRepresentation Patient { get; }
        public Guid RequestId { get; }
        public DateTime Timestamp { get; }
        public string TransactionId { get; }
        public Error Error { get; }
        public DiscoveryResponse Resp { get; }

        public GatewayDiscoveryRepresentation(
            PatientEnquiryRepresentation patient,
            Guid requestId,
            DateTime timestamp,
            string transactionId,
            Error error,
            DiscoveryResponse resp)
        {
            Patient = patient;
            RequestId = requestId;
            Timestamp = timestamp;
            TransactionId = transactionId;
            Error = error;
            Resp = resp;
        }
    }
}