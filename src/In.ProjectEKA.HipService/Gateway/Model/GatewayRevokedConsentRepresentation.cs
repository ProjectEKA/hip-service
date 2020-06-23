using System;
using In.ProjectEKA.HipLibrary.Patient.Model;
using In.ProjectEKA.HipService.Common.Model;

namespace In.ProjectEKA.HipService.Gateway.Model
{
    public class GatewayRevokedConsentRepresentation
    {
        public Guid RequestId { get; }
        public DateTime Timestamp { get; }
        public ConsentUpdateResponse Acknowledgement { get; }
        public Resp Resp { get; }
        public Error Error { get; }
        public GatewayRevokedConsentRepresentation(Guid requestId, DateTime timestamp, ConsentUpdateResponse acknowledgement, Error error, Resp resp)
        {
            RequestId = requestId;
            Timestamp = timestamp;
            Acknowledgement = acknowledgement;
            Resp = resp;
            Error = error;
        }
    }
}