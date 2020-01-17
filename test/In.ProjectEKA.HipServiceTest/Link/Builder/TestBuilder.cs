namespace In.ProjectEKA.HipServiceTest.Link.Builder
{
    using Bogus;

    public static class TestBuilder
    {
        private static Faker faker;
        internal static Faker Faker() => faker ??= new Faker();  
    }
}