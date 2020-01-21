namespace In.ProjectEKA.HipService.Link
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