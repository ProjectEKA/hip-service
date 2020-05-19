namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public class DiscoveryRequest
    {
        public PatientEnquiry Patient { get; }

        public string RequestId { get; }
        public string TransactionId { get; }
        public string Timestamp { get; }

        public DiscoveryRequest(PatientEnquiry patient, string requestId, string transactionId, string timestamp)
        {
            Patient = patient;
            RequestId = requestId;
            TransactionId = transactionId;
            Timestamp = timestamp;
        }
    }
}