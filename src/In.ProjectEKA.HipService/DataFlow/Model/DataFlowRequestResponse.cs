namespace In.ProjectEKA.HipService.DataFlow.Model
{
    public class DataFlowRequestResponse
    {
        public DataFlowRequestResponse(string transactionId, string sessionStatus)
        {
            TransactionId = transactionId;
            SessionStatus = sessionStatus;
        }

        public string TransactionId { get; }

        public string SessionStatus { get; }
    }
}