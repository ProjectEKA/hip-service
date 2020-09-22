namespace In.ProjectEKA.HipService.Patient.Model
{
    using System;
    using HipLibrary.Patient.Model;
    using Hl7.Fhir.Model;

    public class PatientProfileAcknowledgementResponse
    {
        public Guid RequestId { get;}
        public DateTime Timestamp { get; }
        public Acknowledgement Acknowledgement { get; }
        public Resp Resp { get; }
        public Error Error { get; }

        public PatientProfileAcknowledgementResponse(Guid requestId, DateTime timestamp, Acknowledgement acknowledgement, Resp resp, Error error)
        {
            RequestId = requestId;
            Timestamp = timestamp;
            Acknowledgement = acknowledgement;
            Resp = resp;
            Error = error;
        }
    }
}