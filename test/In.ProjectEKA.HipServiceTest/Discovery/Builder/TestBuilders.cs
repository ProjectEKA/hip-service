namespace In.ProjectEKA.HipServiceTest.Discovery.Builder
{
    using Bogus;
    using HipService.Discovery.Patient.Model;

    public static class TestBuilders
    {
        private static Faker faker;
        internal static Faker<Patient> patient()
        {
            return new Faker<Patient>();
        }

        internal static Faker<IdentifierBuilder> identifier()
        {
            return new Faker<IdentifierBuilder>();
        }

        internal static Faker Faker() => faker ??= new Faker();
    }
}