namespace In.ProjectEKA.OtpService.Otp
{
    public class OtpVerificationRequest
    { 
        public string Value { get; }
        
        public OtpVerificationRequest(string value)
        {
            Value = value;
        }
    }
}