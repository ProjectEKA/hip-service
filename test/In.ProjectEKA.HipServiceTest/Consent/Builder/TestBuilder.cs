namespace In.ProjectEKA.HipServiceTest.Consent.Builder
{
    using Bogus;
    using Common.Builder;
    using HipService.Common.Model;
    using HipService.Consent.Model;

    public class TestBuilder
    {
        private static Faker faker = new Faker();
        internal static Faker Faker() => faker ??= new Faker();

        internal static Notification Notification(
            ConsentStatus consentStatus = ConsentStatus.GRANTED)
        {
            return new Notification(ConsentArtefact().Generate().Build(),
                                    faker.Random.Hash(),
                                    faker.Random.Hash(),
                                    consentStatus);
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
            return new Faker<ConsentArtefactBuilder>()
                .RuleFor(c => c.ConsentManager, () => new OrganizationReference("123"));
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