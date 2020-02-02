namespace In.ProjectEKA.HipServiceTest.Discovery.Builder
{
    using Bogus;
    using HipLibrary.Patient.Model.Response;

    public static class TestBuilders
    {
        private static Faker faker;
        internal static Faker<Patient> Patient()
        {
            return new Faker<Patient>();
        }

        internal static Faker<HipService.Discovery.Model.Patient> PatientResponse()
        {
            return new Faker<HipService.Discovery.Model.Patient>();
        }

        internal static Faker<IdentifierBuilder> Identifier()
        {
            return new Faker<IdentifierBuilder>();
        }

        internal static Faker Faker() => faker ??= new Faker();
    }
}