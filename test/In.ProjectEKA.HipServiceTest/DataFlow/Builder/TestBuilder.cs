namespace In.ProjectEKA.HipServiceTest.DataFlow.Builder
{
    using Bogus;
    using Common.Builder;
    using HipService.Common.Model;
    using In.ProjectEKA.HipService.DataFlow;

    public static class TestBuilder
    {
        private static Faker faker;

        internal static Faker Faker() => faker ??= new Faker();

        internal static HealthInformationRequest HealthInformationRequest(string transactionId)
        {
            return new HealthInformationRequest(
                transactionId,
                new Consent(faker.Random.Hash(),
                    faker.Random.Hash()),
                new HiDataRange(faker.Random.Hash(), faker.Random.Hash()),
                faker.Random.Hash());
        }

        internal static Faker<ConsentArtefactBuilder> ConsentArtefact()
        {
            return new Faker<ConsentArtefactBuilder>();
        }

        internal static Faker<DataRequestBuilder> DataRequest()
        {
            return new Faker<DataRequestBuilder>();
        }

        internal static HipService.Consent.Model.Consent Consent()
        {
            return new HipService.Consent.Model.Consent(
                faker.Random.Hash(),
                ConsentArtefact().Generate().Build(),
                faker.Random.Hash(),
                ConsentStatus.GRANTED
            );
        }
    }
}