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
                ConsentStatus.GRANTED, faker.Random.Hash());
        }

        internal static Notification Notification()
        {
            return new Notification(ConsentArtefact().Generate().Build(),
                                    faker.Random.Hash(),
                                    faker.Random.Hash(),
                                    ConsentStatus.GRANTED);
        }
        
        internal static Notification RevokedNotification(string patientId)
        {
            var consentArtefactBuilder = ConsentArtefact().Generate();
            consentArtefactBuilder.Patient = new PatientReference(patientId);
            return new Notification(consentArtefactBuilder.Build(),
                faker.Random.Hash(),
                faker.Random.Hash(),
                ConsentStatus.REVOKED);
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
                ConsentStatus.GRANTED,
                faker.Random.Hash()
            );
        }
    }
}