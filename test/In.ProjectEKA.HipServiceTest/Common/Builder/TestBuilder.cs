namespace In.ProjectEKA.HipServiceTest.Common.Builder
{
    using Bogus;

    public static class TestBuilder
    {
        private static Faker faker;

        internal static Faker Faker()
        {
            return faker ??= new Faker();
        }

        internal static string RandomString()
        {
            return Faker().Random.String();
        }
    }
}