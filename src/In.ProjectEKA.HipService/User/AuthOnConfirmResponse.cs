namespace In.ProjectEKA.HipService.User
{
    using System;
    using HipLibrary.Patient.Model;

    public class AuthOnConfirmResponse
    {
        public AuthOnConfirmResponse(Guid requestId, DateTime timestamp, AuthOnConfirm auth, Error error, Resp resp)
        {
            RequestId = requestId;
            Timestamp = timestamp;
            Auth = auth;
            Error = error;
            Resp = resp;
        }

        public Guid RequestId { get; }
        public DateTime Timestamp { get; }
        public AuthOnConfirm Auth { get; }
        public Error Error { get; }
        public Resp Resp { get; }
    }
}