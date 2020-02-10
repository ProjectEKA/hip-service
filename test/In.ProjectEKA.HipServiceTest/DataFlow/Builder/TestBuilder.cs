namespace In.ProjectEKA.HipServiceTest.DataFlow.Builder
{
    using System;
    using System.Collections.Generic;
    using Bogus;
    using In.ProjectEKA.HipService.DataFlow;

    public static class TestBuilder
    {
        private static Faker faker;
        
        internal static Faker Faker() => faker ??= new Faker();
        
        internal static HealthInformationRequest HealthInformationRequest(string transactionId)
        {
            return new HealthInformationRequest(transactionId,
                new Consent(faker.Random.Hash(),
                faker.Random.Hash()),
                new HiDataRange(faker.Random.Hash(), faker.Random.Hash()),
                faker.Random.Hash());
        }

        internal static Faker<ConsentArtefact> ConsentArtefact()
        {
            return new Faker<ConsentArtefact>();
        //     var consentPurpose = new ConsentPurpose("Encounter", "ENCOUNTER","Some");
        //     var patientReference = new PatientReference("25@ncg");
        //     var hipReference = new HIPReference("10000005", "Xyz centre" );
        //     var hiTypes = new List<HiType> {HiType.Condition};
        //     var consentPermission = new ConsentPermission(
        //         AccessMode.View,
        //         new AccessPeriod(DateTime.Now, DateTime.Today),
        //         new DataFrequency(DataFrequencyUnit.Day, 1, 1),
        //         DateTime.Today);
        //     var grantedContexts = new List<GrantedContext> {new GrantedContext("12345",
        //         "batman@tmh")};   
        //     
        //     return new ConsentArtefact(consentId,
        //         DateTime.Now,
        //         consentPurpose,
        //         patientReference,
        //         hipReference,
        //         hiTypes,
        //         consentPermission,
        //         grantedContexts);
        }
    }
}