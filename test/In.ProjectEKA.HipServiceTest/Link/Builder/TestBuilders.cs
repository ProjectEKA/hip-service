namespace In.ProjectEKA.HipServiceTest.Link.Builder
{
    using In.ProjectEKA.HipService.Link;
    using Bogus;
    using HipLibrary.Patient.Model;

    public static class TestBuilders
    {
        private static Faker faker;

        internal static Faker Faker() => faker ??= new Faker();

        internal static PatientLinkReferenceRequest GetFakeLinkRequest()
        {
            return new PatientLinkReferenceRequest(
                faker.Random.Hash(),
                new LinkReference(
                    faker.Random.Hash(),
                    faker.Random.Hash(),
                    new[] {new CareContextEnquiry(faker.Random.Hash())}));
        }
    }
}