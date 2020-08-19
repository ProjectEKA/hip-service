namespace In.ProjectEKA.HipService.Link
{
    public class OtpVerificationRequest
    {
        public OtpVerificationRequest(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}