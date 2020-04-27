namespace In.ProjectEKA.HipService.DataFlow
{
    public class HealthInformationTransactionResponse
    {
        public string AcknowledgementId { get; }
        
        public HealthInformationTransactionResponse(string transactionId)
        {
            AcknowledgementId = transactionId;
        }
    }
}