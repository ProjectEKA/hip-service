using System;
using System.Collections.Generic;
using In.ProjectEKA.DefaultHipTest.DataFlow.Builder;
using In.ProjectEKA.HipLibrary.Patient.Model;
using Xunit.Abstractions;

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
        private readonly ITestOutputHelper testOutputHelper;
        static readonly HiTypeDataMap HiTypeDataMap = new HiTypeDataMap();
        private readonly Collect collect
            = new Collect(HiTypeDataMap);

        public CollectTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        private async void ReturnEntries()
        {
            var grantedContexts = new List<GrantedContext>() {new GrantedContext("RVH/NCC-1701", "NCP1006")};
            var hiDataRange = new HiDataRange("2019-01-01", "2020-01-01");
            var hiTypes = new List<HiType>()
                {HiType.Condition, HiType.Observation, HiType.DiagnosticReport, HiType.MedicationRequest};
            var dataRequest = new DataRequest(grantedContexts, hiDataRange, "/someUrl", hiTypes, "someTxnId", null);
            var entries = await collect.CollectData(dataRequest);

            var bundles = entries.Map(s => s.Bundles);
            bundles.ValueOrDefault().Count().Should().Be(3);
        }
    }
}