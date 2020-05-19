using System;
using Hl7.Fhir.Model;

namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public class DiscoveryRepresentation
    {
        public PatientEnquiryRepresentation Patient { get; }

        public DiscoveryRepresentation(PatientEnquiryRepresentation patient)
        {
            Patient = patient;
        }
    }

    public class GatewayDiscoveryRepresentation
    {
        public PatientEnquiryRepresentation Patient { get; }
        public Guid RequestId { get; }
        public string Timestamp { get; }
        public string TransactionId { get; }
        public Error Error { get; }
        public Resp Resp { get; }

        public GatewayDiscoveryRepresentation(PatientEnquiryRepresentation patient, Guid requestId,
            string timestamp, string transactionId, Error error, Resp resp)
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