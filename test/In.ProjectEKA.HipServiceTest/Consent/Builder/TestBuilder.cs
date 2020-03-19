namespace In.ProjectEKA.HipServiceTest.Consent.Builder
{
    using Bogus;
    using Common.Builder;
    using HipService.Common.Model;
    using HipService.Consent;
    using HipService.Consent.Model;

    public class TestBuilder
    {
        private static Faker faker = new Faker();
        internal static Faker Faker() => faker ??= new Faker();

        internal static ConsentArtefactRequest ConsentArtefactRequest()
        {
            return new ConsentArtefactRequest(faker.Random.Hash(),
                ConsentArtefact().Generate().Build(),
                ConsentStatus.GRANTED);
        }

        private static Faker<ConsentArtefactBuilder> ConsentArtefact()
        {
            return new Faker<ConsentArtefactBuilder>();
        }

        internal static Consent Consent()
        {
            return new Consent(faker.Random.Hash(),
                ConsentArtefact().Generate().Build(),
                faker.Random.Hash(),
                ConsentStatus.GRANTED
            );
        }
    }
}