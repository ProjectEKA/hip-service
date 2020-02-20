namespace In.ProjectEKA.HipService.DataFlow
{
    public class LinkDataResponse
    {
        public string TransactionId { get; }
        public Entry Entry { get; }

        public LinkDataResponse(string transactionId, Entry entry)
        {
            TransactionId = transactionId;
            Entry = entry;
        }
    }
}