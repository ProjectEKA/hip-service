namespace In.ProjectEKA.OtpService.Otp
{
    public class OtpProperties
    {
        public int ExpiryInMinutes { get; }

        public OtpProperties(int expiryInMinutes)
        {
            ExpiryInMinutes = expiryInMinutes;
        }
    }
}