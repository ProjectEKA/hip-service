namespace In.ProjectEKA.HipService.Patient.Model
{
    using System;

    public class PatientProfile
    {
        public string RequestId { get;}
        public DateTime Timestamp { get; }
        public PatientDetails PatientDetails { get; }
        public PatientProfile(string requestId, DateTime timestamp, PatientDetails profile)
        {
            RequestId = requestId;
            Timestamp = timestamp;
            PatientDetails = profile;
        }
    }
}