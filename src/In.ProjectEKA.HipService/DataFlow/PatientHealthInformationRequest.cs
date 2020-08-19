namespace In.ProjectEKA.HipService.DataFlow
{
    using System;

    public class PatientHealthInformationRequest
    {
        public PatientHealthInformationRequest(string transactionId,
            string requestId,
            DateTime timestamp,
            HIRequest hiRequest)
        {
            TransactionId = transactionId;
            RequestId = requestId;
            Timestamp = timestamp;
            HiRequest = hiRequest;
        }

        public string TransactionId { get; }
        public string RequestId { get; }
        public DateTime Timestamp { get; }
        public HIRequest HiRequest { get; }
    }
}