using Bogus;
using otp_server.Otp;

namespace otp_server_test.Otp.Builder
{
    public static class TestBuilder
    {
        private static Faker faker;
        internal static Faker Faker() => faker ??= new Faker();
        
        internal static Faker<OtpGenerationRequest> otpGenerationRequest()
        {
            return new Faker<OtpGenerationRequest>();
        }
        
        internal static Faker<OtpVerificationRequest> otpVerificationRequest()
        {
            return new Faker<OtpVerificationRequest>();
        }
    }
}