namespace In.ProjectEKA.HipService.Link
{
    public class LinkReferenceRequest
    {
        public LinkReferenceRequest(string transactionId, PatientLinkReference patient, string requestId)
        {
            TransactionId = transactionId;
            Patient = patient;
            RequestId = requestId;
        }

        public string TransactionId { get; }

        public string RequestId { get; }

        public PatientLinkReference Patient { get; }
    }
}