namespace In.ProjectEKA.HipServiceTest.Discovery.Builder
{
    using Bogus;
    using Patient = HipLibrary.Patient.Model.Patient;

    public static class TestBuilders
    {
        private static Faker faker;

        internal static Faker<Patient> Patient()
        {
            return new Faker<Patient>();
        }

        internal static Faker<IdentifierBuilder> Identifier()
        {
            return new Faker<IdentifierBuilder>();
        }

        internal static Faker Faker() => faker ??= new Faker();

        public static string RandomString()
        {
            return Faker().Random.String();
        }
    }
}