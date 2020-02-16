using In.ProjectEKA.DefaultHipTest.DataFlow.Builder;

namespace In.ProjectEKA.HipServiceTest.DataFlow.Builder
{
    using Bogus;

    public static class TestBuilder
    {
        private static Faker faker;
        
        internal static Faker Faker() => faker ??= new Faker();

        internal static Faker<DataRequestBuilder> DataRequest()
        {
            return new Faker<DataRequestBuilder>();
        }
    }
}