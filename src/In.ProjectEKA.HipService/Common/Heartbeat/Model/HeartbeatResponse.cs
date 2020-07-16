using System;
using In.ProjectEKA.HipLibrary.Patient.Model;

namespace In.ProjectEKA.HipService.Common.Heartbeat.Model
{
    public class HeartbeatResponse
    {
        public DateTime Timestamp  { get; }
        
        public string Status { get;}
        
        public Error Error { get; }

        public HeartbeatResponse(DateTime timestamp, string status, Error error)
        {
            Timestamp = timestamp;
            Status = status;
            Error = error;
        }
    }
}