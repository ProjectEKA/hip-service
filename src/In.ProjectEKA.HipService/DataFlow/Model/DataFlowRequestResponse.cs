namespace In.ProjectEKA.HipService.DataFlow.Model
{
    public class DataFlowRequestResponse
    {
        public string TransactionId { get; }
        
        public string SessionStatus {get;}

        public DataFlowRequestResponse(string transactionId, string sessionStatus)
        {
            TransactionId = transactionId;
            SessionStatus = sessionStatus;
        }
    }
}