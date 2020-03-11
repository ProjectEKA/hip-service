namespace In.ProjectEKA.OtpService.Otp
{
    public class OtpGenerationRequest
    {
        public string SessionId { get; set; }

        public Communication Communication { get; set; }

        public OtpGenerationRequest(string sessionId, Communication communication)
        {
            SessionId = sessionId;
            Communication = communication;
        }
    }
}