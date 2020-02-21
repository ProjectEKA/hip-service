namespace In.ProjectEKA.HipServiceTest.DataFlow.Builder
{
    using Bogus;
    using In.ProjectEKA.HipService.Common.Model;
    using In.ProjectEKA.HipService.DataFlow;
    using In.ProjectEKA.HipServiceTest.Common.Builder;
    using Consent = In.ProjectEKA.HipService.Consent.Model.Consent;

    public static class TestBuilder
    {
        private static Faker faker;

        internal static Faker Faker()
        {
            return faker ??= new Faker();
        }

        internal static HealthInformationRequest HealthInformationRequest(string transactionId)
        {
            return new HealthInformationRequest(
                transactionId,
                new HipService.DataFlow.Consent(faker.Random.Hash(),
                    faker.Random.Hash()),
                new HiDataRange(faker.Random.Hash(), faker.Random.Hash()),
                faker.Random.Hash(),
                TestBuilder.KeyMaterial().Generate().Build());
        }

        internal static Faker<ConsentArtefactBuilder> ConsentArtefact()
        {
            return new Faker<ConsentArtefactBuilder>();
        }

        internal static Faker<DataRequestBuilder> DataRequest()
        {
            return new Faker<DataRequestBuilder>();
        }
        
        internal static Faker<KeyMaterialBuilder> KeyMaterial()
        {
            return new Faker<KeyMaterialBuilder>();
        }

        internal static Consent Consent()
        {
            return new Consent(
                faker.Random.Hash(),
                ConsentArtefact().Generate().Build(),
                faker.Random.Hash(),
                ConsentStatus.GRANTED
            );
        }
    }
}