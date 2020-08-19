namespace In.ProjectEKA.HipService.DataFlow
{
    public class HealthInformationTransactionResponse
    {
        public HealthInformationTransactionResponse(string transactionId)
        {
            AcknowledgementId = transactionId;
        }

        public string AcknowledgementId { get; }
    }
}