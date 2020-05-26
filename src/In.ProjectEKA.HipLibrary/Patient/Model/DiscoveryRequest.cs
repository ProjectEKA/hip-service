using System;

namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public class DiscoveryRequest
    {
        public PatientEnquiry Patient { get; }

        public string RequestId { get; }

        public string TransactionId { get; }

        public DateTime Timestamp { get; }

        public DiscoveryRequest(PatientEnquiry patient, string requestId, string transactionId, DateTime timestamp)
        {
            Patient = patient;
            RequestId = requestId;
            TransactionId = transactionId;
            Timestamp = timestamp;
        }
    }
}