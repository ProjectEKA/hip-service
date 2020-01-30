namespace In.ProjectEKA.DefaultHip.Link
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