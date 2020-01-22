namespace In.ProjectEKA.OtpService.Otp
{
    public class OtpVerificationRequest
    {
        public string SessionID { get; }
        public string Value { get; }
        
        public OtpVerificationRequest(string sessionId, string value)
        {
            SessionID = sessionId;
            Value = value;
        }
    }
}