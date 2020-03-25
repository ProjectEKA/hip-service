namespace In.ProjectEKA.HipService.DataFlow
{
    public class HealthInformationResponse
    {
        public string TransactionId { get; }
        public string Content { get; }

        public HealthInformationResponse(string transactionId, string content)
        {
            TransactionId = transactionId;
            Content = content;
        }
    }
}