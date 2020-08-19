namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    using System;

    public class DiscoveryRequest
    {
        public DiscoveryRequest(PatientEnquiry patient, string requestId, string transactionId, DateTime timestamp)
        {
            Patient = patient;
            RequestId = requestId;
            TransactionId = transactionId;
            Timestamp = timestamp;
        }

        public PatientEnquiry Patient { get; }

        public string RequestId { get; }

        public string TransactionId { get; }

        public DateTime Timestamp { get; }
    }
}