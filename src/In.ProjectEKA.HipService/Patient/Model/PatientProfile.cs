namespace In.ProjectEKA.HipService.Patient.Model
{
    using System;

    public class PatientProfile
    {
        public string RequestId { get;}
        public DateTime Timestamp { get; }
        public HipDetails HipDetails { get; }
        public PatientDetails Patient { get; }

        public PatientProfile(string requestId, DateTime timestamp, HipDetails hipDetails, PatientDetails patient)
        {
            RequestId = requestId;
            Timestamp = timestamp;
            HipDetails = hipDetails;
            Patient = patient;
        }
    }
}