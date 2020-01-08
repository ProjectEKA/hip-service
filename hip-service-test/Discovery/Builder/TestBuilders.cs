using Bogus;

namespace hip_service_test.Discovery.Builder
{
    public static class TestBuilders
    {
        private static Faker faker;
        internal static Faker<hip_service.Discovery.Patient.Model.Patient> patient()
        {
            return new Faker<hip_service.Discovery.Patient.Model.Patient>();
        }

        internal static Faker<IdentifierBuilder> identifier()
        {
            return new Faker<IdentifierBuilder>();
        }

        internal static Faker Faker() => faker ??= new Faker();
    }
}