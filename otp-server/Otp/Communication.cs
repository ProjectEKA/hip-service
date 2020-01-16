namespace OtpServer.Otp
{
    public class Communication
    {
        public string Mode { get; }
        public string Value { get; }

        public Communication(string mode, string value)
        {
            Mode = mode;
            Value = value;
        }
    }
}