namespace In.ProjectEKA.DefaultHipTest.DataFlow
{
    using System.Linq;
    using FluentAssertions;
    using In.ProjectEKA.DefaultHip.DataFlow;
    using In.ProjectEKA.HipServiceTest.DataFlow.Builder;
    using Optional.Unsafe;
    using Xunit;

    [Collection("Collect Tests")]
    public class CollectTest
    {
        static readonly HiTypeDataMap HiTypeDataMap = new HiTypeDataMap();
        private readonly Collect collect
            = new Collect(HiTypeDataMap);

        [Fact]
        private async void ReturnEntries()
        {
            var dataRequest = TestBuilder.DataRequest(TestBuilder.Faker().Random.Hash());
            var entries = await collect.CollectData(dataRequest);

            var bundles = entries.Map(s => s.Bundles);
            bundles.ValueOrDefault().Count().Should().Be(3);
        }
    }
}