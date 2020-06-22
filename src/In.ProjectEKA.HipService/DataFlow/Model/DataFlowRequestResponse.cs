namespace In.ProjectEKA.HipService.DataFlow.Model
{
    public class DataFlowRequestResponse
    {
        public string TransactionId { get; }
        
        public DataFlowRequestStatus SessionStatus {get;}

        public DataFlowRequestResponse(string transactionId, DataFlowRequestStatus sessionStatus)
        {
            TransactionId = transactionId;
            SessionStatus = sessionStatus;
        }
    }
}