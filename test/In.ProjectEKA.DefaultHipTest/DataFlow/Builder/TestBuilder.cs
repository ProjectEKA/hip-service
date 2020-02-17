namespace In.ProjectEKA.HipServiceTest.DataFlow.Builder
{
    using Bogus;
    using In.ProjectEKA.DefaultHipTest.DataFlow.Builder;

    public static class TestBuilder
    {
        private static Faker faker;

        internal static Faker Faker()
        {
            return faker ??= new Faker();
        }

        internal static Faker<DataRequestBuilder> DataRequest()
        {
            return new Faker<DataRequestBuilder>();
        }
    }
}