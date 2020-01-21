using Bogus;
using OtpServer.Otp;

namespace OtpServerTest.Otp.Builder
{
    public static class TestBuilder
    {
        private static Faker faker;
        internal static Faker Faker() => faker ??= new Faker();
    }
}