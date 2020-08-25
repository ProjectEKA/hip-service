namespace In.ProjectEKA.HipService.Link.Model
{
    using System;
    using HipLibrary.Patient.Model;

    public class AuthOnInitRequest
    {
        public AuthOnInitRequest(string requestId, DateTime timestamp, AuthInit authInit, Error error, Resp resp)
        {
            RequestId = requestId;
            Timestamp = timestamp;
            AuthInit = authInit;
            Error = error;
            Resp = resp;
        }

        public string RequestId { get; }
        public DateTime Timestamp { get; }
        public AuthInit AuthInit { get; }
        public Error Error { get; }
        public Resp Resp { get; }
    }
}