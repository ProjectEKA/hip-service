namespace In.ProjectEKA.HipService.Link
{
    public class LinkPatientRequest
    {
        public string RequestId { get; }
        public string Timestamp { get; }
        public LinkConfirmation Confirmation { get; }

        public LinkPatientRequest(string requestId, string timestamp, LinkConfirmation confirmation)
        {
            RequestId = requestId;
            Timestamp = timestamp;
            Confirmation = confirmation;
        }
    }
}