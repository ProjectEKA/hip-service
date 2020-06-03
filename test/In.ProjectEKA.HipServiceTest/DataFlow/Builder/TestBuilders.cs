using Bogus;

namespace In.ProjectEKA.TMHHipTest.DataFlow.Builder
{
    public static class TestBuilders
    {
        private static Faker faker;

        internal static Faker<HipLibrary.Patient.Model.Patient> Patient()
        {
            return new Faker<HipLibrary.Patient.Model.Patient>();
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