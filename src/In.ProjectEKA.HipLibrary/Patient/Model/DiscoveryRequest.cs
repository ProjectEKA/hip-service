namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public class DiscoveryRequest
    {
        public PatientEnquiry Patient { get; }

        public string TransactionId { get; }

        public DiscoveryRequest(PatientEnquiry patient, string transactionId)
        {
            Patient = patient;
            TransactionId = transactionId;
        }
    }
}