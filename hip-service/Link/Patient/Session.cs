using hip_library.Patient.models.dto;

namespace hip_service.OTP
{
    public class Session
    {
        public string SessionId { get; set; }
        public Communication Communication { get; set; }

        public Session(string sessionId, Communication communication)
        {
            SessionId = sessionId;
            Communication = communication;
        }
    }
}