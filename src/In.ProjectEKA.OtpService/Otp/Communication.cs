namespace In.ProjectEKA.OtpService.Otp
{
    public class Communication
    {
        public string Mode { get; set; }
        public string Value { get; set; }

        public Communication(string mode, string value)
        {
            Mode = mode;
            Value = value;
        }
    }
}