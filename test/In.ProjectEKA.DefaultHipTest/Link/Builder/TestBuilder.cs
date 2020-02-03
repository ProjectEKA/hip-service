using Bogus;
using In.ProjectEKA.DefaultHip.Link;

namespace In.ProjectEKA.DefaultHipTest.Link.Builder
{
    public static class TestBuilder
    {
        private static Faker faker;
        internal static Faker Faker() => faker ??= new Faker();
        internal static Faker<OtpMessage> otpMessage()
        {
            return new Faker<OtpMessage>();
        }
    }
}