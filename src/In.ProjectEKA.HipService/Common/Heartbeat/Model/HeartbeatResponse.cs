namespace In.ProjectEKA.HipService.Common.Heartbeat.Model
{
    using System;
    using HipLibrary.Patient.Model;

    public class HeartbeatResponse
    {
        public HeartbeatResponse(DateTime timestamp, string status, Error error)
        {
            Timestamp = timestamp;
            Status = status;
            Error = error;
        }

        public DateTime Timestamp { get; }

        public string Status { get; }

        public Error Error { get; }
    }
}