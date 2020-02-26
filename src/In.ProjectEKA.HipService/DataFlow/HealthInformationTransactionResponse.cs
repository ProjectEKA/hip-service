namespace In.ProjectEKA.HipService.DataFlow
{
    public class HealthInformationTransactionResponse
    {
        public string TransactionId { get; }
        
        public HealthInformationTransactionResponse(string transactionId)
        {
            TransactionId = transactionId;
        }
    }
}