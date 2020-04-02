using System.Collections.Generic;
using In.ProjectEKA.HipLibrary.Patient.Model;

namespace In.ProjectEKA.DefaultHipTest.DataFlow
{
    using System.Linq;
    using FluentAssertions;
    using In.ProjectEKA.DefaultHip.DataFlow;
    using Optional.Unsafe;
    using Xunit;

    [Collection("Collect Tests")]
    public class CollectTest
    {
        private static readonly HiTypeDataMap HiTypeDataMap = new HiTypeDataMap();

        private readonly Collect collect
            = new Collect(HiTypeDataMap);

        [Fact]
        private async void ShouldReturnEntries()
        {
            var grantedContexts = new List<GrantedContext>() {new GrantedContext("RVH1003", "BI-KTH-12.05.0024"), new GrantedContext("RVH1003", "NCP1008")};
            var hiDataRange = new HiDataRange("2017-12-01T15:43:00.000+0000", "2020-03-31T15:43:19.279+0000"); 
            var hiTypes = new List<HiType>()
                {HiType.Condition, HiType.Observation, HiType.DiagnosticReport, HiType.MedicationRequest};
            var dataRequest = new DataRequest(grantedContexts, hiDataRange, "/someUrl", hiTypes, "someTxnId", null);
            var entries = await collect.CollectData(dataRequest);

            var bundles = entries.Map(s => s.Bundles);
            bundles.ValueOrDefault().Count().Should().Be(7);
        }
        
        [Fact]
        private async void ShouldReturnEntriesForDateRange()
        {
            var grantedContexts = new List<GrantedContext>() {new GrantedContext("RVH/NCC-1701", "SC1701"), new GrantedContext("RVH/NCC-1701", "NCP1006")};
            var hiDataRange = new HiDataRange("2019-06-02T00:00:00.000+0530", "2020-03-31T15:43:19.279+0000"); 
            var hiTypes = new List<HiType>()
                {HiType.Condition, HiType.Observation, HiType.DiagnosticReport, HiType.MedicationRequest};
            var dataRequest = new DataRequest(grantedContexts, hiDataRange, "/someUrl", hiTypes, "someTxnId", null);
            var entries = await collect.CollectData(dataRequest);

            var bundles = entries.Map(s => s.Bundles);
            bundles.ValueOrDefault().Count().Should().Be(2);
        }
        
        [Fact]
        private async void ShouldReturnEntriesForCareContext()
        {
            var grantedContexts = new List<GrantedContext>() {new GrantedContext("RVH1003", "BI-KTH-12.05.0024")};
            var hiDataRange = new HiDataRange("2017-12-01T15:43:00.000+0000", "2020-03-31T15:43:19.279+0000"); 
            var hiTypes = new List<HiType>()
                {HiType.Condition, HiType.Observation, HiType.DiagnosticReport, HiType.MedicationRequest};
            var dataRequest = new DataRequest(grantedContexts, hiDataRange, "/someUrl", hiTypes, "someTxnId", null);
            var entries = await collect.CollectData(dataRequest);

            var bundles = entries.Map(s => s.Bundles);
            bundles.ValueOrDefault().Count().Should().Be(1);
        }
    }
}