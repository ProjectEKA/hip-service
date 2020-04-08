namespace In.ProjectEKA.DefaultHipTest.DataFlow
{
    using System.Collections.Generic;
    using System.Linq;
    using DefaultHip.DataFlow;
    using FluentAssertions;
    using HipLibrary.Patient.Model;
    using Optional.Unsafe;
    using Xunit;

    [Collection("Collect Tests")]
    public class CollectTest
    {
        private readonly Collect collect = new Collect("demoPatientCareContextDataMap.json");

        [Fact]
        private async void ReturnEntries()
        {
            const string consentManagerId = "ConsentManagerId";
            var grantedContexts = new List<GrantedContext>
            {
                new GrantedContext("RVH1003", "BI-KTH-12.05.0024"),
                new GrantedContext("RVH1003", "NCP1008")
            };
            var hiDataRange = new HiDataRange("2017-12-01T15:43:00.000+0000", "2020-03-31T15:43:19.279+0000");
            var hiTypes = new List<HiType>
            {
                HiType.Condition,
                HiType.Observation,
                HiType.DiagnosticReport,
                HiType.MedicationRequest
            };
            var dataRequest = new DataRequest(grantedContexts,
                hiDataRange,
                "/someUrl",
                hiTypes,
                "someTxnId",
                null,
                consentManagerId);

            var entries = await collect.CollectData(dataRequest);

            var bundles = entries.Map(s => s.Bundles);
            bundles.ValueOrDefault().Count().Should().Be(7);
        }
    }
}