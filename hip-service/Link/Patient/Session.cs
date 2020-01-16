
using hip_service.Link.Patient;

namespace hip_service.OTP
{
    public class Session
    {
        public string SessionId { get; }
        public Communication Communication { get; }

        public Session(string sessionId, Communication communication)
        {
            SessionId = sessionId;
            Communication = communication;
        }
    }
}