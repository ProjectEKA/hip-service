namespace In.ProjectEKA.HipService.Link.Model
{
    using System;
    using HipLibrary.Patient.Model;

    public class AuthOnInitRequest
    {
        public AuthOnInitRequest(string requestId, DateTime timestamp, Auth auth, Error error, Resp resp)
        {
            RequestId = requestId;
            Timestamp = timestamp;
            Auth = auth;
            Error = error;
            Resp = resp;
        }

        public string RequestId { get; }
        public DateTime Timestamp { get; }
        public Auth Auth { get; }
        public Error Error { get; }
        public Resp Resp { get; }
    }
}