namespace In.ProjectEKA.HipService.Link
{
    public class LinkPatientRequest
    {
        public LinkPatientRequest(string requestId, string timestamp, LinkConfirmation confirmation)
        {
            RequestId = requestId;
            Timestamp = timestamp;
            Confirmation = confirmation;
        }

        public string RequestId { get; }
        public string Timestamp { get; }
        public LinkConfirmation Confirmation { get; }
    }
}