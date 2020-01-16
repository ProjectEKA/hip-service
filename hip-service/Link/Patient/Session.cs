
namespace hip_service.OTP
{
    public class Session
    {
        public string SessionId { get; }
        private Communication Communication { get; }

        public Session(string sessionId, Communication communication)
        {
            SessionId = sessionId;
            Communication = communication;
        }
    }
}