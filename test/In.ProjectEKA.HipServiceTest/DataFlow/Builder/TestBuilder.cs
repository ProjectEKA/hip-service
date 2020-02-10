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
        }
    }
}