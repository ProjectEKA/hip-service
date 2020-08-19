namespace In.ProjectEKA.HipService.Gateway.Model
{
    using System;
    using Common.Model;
    using HipLibrary.Patient.Model;

    public class GatewayRevokedConsentRepresentation
    {
        public GatewayRevokedConsentRepresentation(Guid requestId, DateTime timestamp,
            ConsentUpdateResponse acknowledgement, Error error, Resp resp)
        {
            RequestId = requestId;
            Timestamp = timestamp;
            Acknowledgement = acknowledgement;
            Resp = resp;
            Error = error;
        }

        public Guid RequestId { get; }
        public DateTime Timestamp { get; }
        public ConsentUpdateResponse Acknowledgement { get; }
        public Resp Resp { get; }
        public Error Error { get; }
    }
}