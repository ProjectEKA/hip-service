using Hl7.Fhir.Utility;

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
        private async void ReturnEntriesForHina()
        {
            const string consentId = "ConsentId";
            const string consentManagerId = "ConsentManagerId";
            var grantedContexts = new List<GrantedContext>
            {
                new GrantedContext("RVH1003", "BI-KTH-12.05.0024"),
                new GrantedContext("RVH1003", "NCP1008")
            };
            
            var dateRange = new DateRange("2017-12-01T15:43:00.818234", "2021-12-31T15:43:00.818234");
            var hiTypes = new List<HiType>
            {
                HiType.Condition,
                HiType.Observation,
                HiType.DiagnosticReport,
                HiType.MedicationRequest,
                HiType.DocumentReference,
                HiType.Prescription,
                HiType.DischargeSummary,
                HiType.OPConsultation
            };
            var dataRequest = new DataRequest(grantedContexts,
                dateRange,
                "/someUrl",
                hiTypes,
                "someTxnId",
                null,
                consentManagerId,
                consentId,
                "sometext");

            var entries = await collect.CollectData(dataRequest);
            entries.ValueOrDefault().CareBundles.Count().Should().Be(18);
        }

        [Fact]
        private async void ReturnEntriesForNavjot()
        {
            const string consentId = "ConsentId";
            const string consentManagerId = "ConsentManagerId";
            var grantedContexts = new List<GrantedContext>
            {
                new GrantedContext("RVH1002", "NCP1007"),
                new GrantedContext("RVH1002", "RV-MHD-01.17.0024")
            };
            var dateRange = new DateRange("2013-12-01T15:43:00.000+0000", "2021-12-31T15:43:19.279+0000");
            var hiTypes = new List<HiType>
            {
                HiType.Condition,
                HiType.Observation,
                HiType.DiagnosticReport,
                HiType.MedicationRequest,
                HiType.DocumentReference,
                HiType.Prescription,
                HiType.DischargeSummary
            };
            var dataRequest = new DataRequest(grantedContexts,
                dateRange,
                "/someUrl",
                hiTypes,
                "someTxnId",
                null,
                consentManagerId,
                consentId,
                "sometext");

            var entries = await collect.CollectData(dataRequest);
            entries.ValueOrDefault().CareBundles.Count().Should().Be(17);
        }
    }
}