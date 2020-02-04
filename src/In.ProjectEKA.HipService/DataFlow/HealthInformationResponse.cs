namespace In.ProjectEKA.HipService.DataFlow
{
    public class HealthInformationResponse
    {
        public string TransactionId { get; }
        
        public HealthInformationResponse(string transactionId)
        {
            TransactionId = transactionId;
        }
    }
}