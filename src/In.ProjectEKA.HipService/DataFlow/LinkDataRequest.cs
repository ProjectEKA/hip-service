namespace In.ProjectEKA.HipService.DataFlow
{
    public class LinkDataRequest
    {
        public string TransactionId { get; }

        public LinkDataRequest(string transactionId)
        {
            TransactionId = transactionId;
        }
    }
}