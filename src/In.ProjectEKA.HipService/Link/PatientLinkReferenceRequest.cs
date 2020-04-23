namespace In.ProjectEKA.HipService.Link
{
    public class PatientLinkReferenceRequest
    {
        public string TransactionId { get; }
        
        public string RequestId { get; }

        public LinkReference Patient { get; }

        public PatientLinkReferenceRequest(string transactionId, LinkReference patient, string requestId)
        {
            TransactionId = transactionId;
            Patient = patient;
            RequestId = requestId;
        }
    }
}