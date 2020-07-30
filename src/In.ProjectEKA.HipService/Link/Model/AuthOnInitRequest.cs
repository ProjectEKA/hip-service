namespace In.ProjectEKA.HipService.Link.Model
{
    using System;
    using In.ProjectEKA.HipLibrary.Patient.Model;

    public class AuthOnInitRequest
    {
        public string RequestId { get; }
        public DateTime Timestamp { get; }
        public AuthInit AuthInit { get; }
        public Error Error { get; }
        public Resp Resp { get; }
        
        public AuthOnInitRequest(string requestId, DateTime timestamp, AuthInit authInit, Error error, Resp resp)
        {
            RequestId = requestId;
            Timestamp = timestamp;
            AuthInit = authInit;
            Error = error;
            Resp = resp;
        }

    }
}