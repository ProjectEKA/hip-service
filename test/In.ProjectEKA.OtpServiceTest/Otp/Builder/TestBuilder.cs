using Bogus;

namespace In.ProjectEKA.OtpServiceTest.Otp.Builder
{
    public static class TestBuilder
    {
        private static Faker faker;
        internal static Faker Faker() => faker ??= new Faker();
    }
}