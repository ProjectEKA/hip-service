namespace In.ProjectEKA.HipService.DataFlow.Model
{
    using System.ComponentModel.DataAnnotations;

    public class DataFlowRequest
    {
        public DataFlowRequest()
        {
        }

        public DataFlowRequest(string transactionId, HealthInformationRequest healthInformationRequest)
        {
            TransactionId = transactionId;
            HealthInformationRequest = healthInformationRequest;
        }

        [Key]
        public string TransactionId { get; }

        public HealthInformationRequest HealthInformationRequest { get; }
    }
}