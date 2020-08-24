namespace In.ProjectEKA.HipService.Gateway.Model
{
    using System;
    using HipLibrary.Patient.Model;

    public class GatewayLinkConfirmResponse
    {
        public GatewayLinkConfirmResponse(Guid requestId,
            DateTime timestamp,
            LinkConfirmationRepresentation patient,
            Error error,
            Resp resp)
        {
            RequestId = requestId;
            Timestamp = timestamp;
            Patient = patient;
            Error = error;
            Resp = resp;
        }

        public Guid RequestId { get; }

        public DateTime Timestamp { get; }

        public LinkConfirmationRepresentation Patient { get; }

        public Error Error { get; }

        public Resp Resp { get; }
    }
}