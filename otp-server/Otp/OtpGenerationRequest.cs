namespace otp_server.Otp
{
    public class OtpGenerationRequest
    { 
        public string SessionId { get; set; }
        public Communication Communication { get; set; }

        public OtpGenerationRequest()
        {
        }

        public OtpGenerationRequest(string sessionId, Communication communication)
        {
            SessionId = sessionId;
            Communication = communication;
        }
    }
}