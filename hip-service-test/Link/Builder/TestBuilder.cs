using Bogus;

namespace hip_service_test.Link.Builder
{
    public static class TestBuilder
    {
        private static Faker faker;
        internal static Faker Faker() => faker ??= new Faker();  
    }
}