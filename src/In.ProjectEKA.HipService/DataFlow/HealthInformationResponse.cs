namespace In.ProjectEKA.HipService.DataFlow
{
    public class HealthInformationResponse
    {
        public string TransactionId { get; }
        public Entry Entry { get; }

        public HealthInformationResponse(string transactionId, Entry entry)
        {
            TransactionId = transactionId;
            Entry = entry;
        }
    }
}