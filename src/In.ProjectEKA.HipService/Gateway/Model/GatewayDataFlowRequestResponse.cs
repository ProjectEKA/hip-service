namespace In.ProjectEKA.HipService.Gateway.Model
{
    using System;
    using HipLibrary.Patient.Model;
    using HipService.DataFlow.Model;

    public class GatewayDataFlowRequestResponse
    {
        public Guid RequestId { get; }
        public DateTime Timestamp { get; }
        public DataFlowRequestResponse HiRequest   { get; }
        public Error Error { get; }
        public Resp Resp { get; }

        public GatewayDataFlowRequestResponse(
            Guid requestId,
            DateTime timestamp,
            DataFlowRequestResponse hiRequest,
            Error error,
            Resp resp)
        {
            RequestId = requestId;
            Timestamp = timestamp;
            HiRequest = hiRequest;
            Error = error;
            Resp = resp;
        }
    }
}