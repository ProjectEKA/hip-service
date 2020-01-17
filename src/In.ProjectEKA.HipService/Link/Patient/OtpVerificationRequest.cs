namespace hip_service.Link.Patient
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