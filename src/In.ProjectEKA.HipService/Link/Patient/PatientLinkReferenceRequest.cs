namespace hip_service.Link.Patient
{
    public class PatientLinkReferenceRequest
    {
        public string TransactionId { get; }
        
        public LinkReference Patient { get; }

        public PatientLinkReferenceRequest(string transactionId, LinkReference patient)
        {
            TransactionId = transactionId;
            Patient = patient;
        }

    }
}