using System.Linq;
using FluentAssertions;
using In.ProjectEKA.DefaultHip.DataFlow;
using In.ProjectEKA.HipServiceTest.DataFlow.Builder;
using Optional.Unsafe;
using Xunit;

namespace In.ProjectEKA.DefaultHipTest.DataFlow
{
    [Collection("Collect Tests")]
    public class CollectTest
    {
        private readonly Collect collect 
            = new Collect("observation.json");
        
        [Fact]
        private async  void ReturnEntries()
        {
            var dataRequest = TestBuilder.DataRequest().Generate().Build();

            var entries = await collect.CollectData(dataRequest);
            
            var bundles = entries.Map(s => s.Bundles);
            bundles.ValueOrDefault().Count().Should().Be(1);
        }
    }
}