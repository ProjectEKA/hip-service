using In.ProjectEKA.HipService.Link;

namespace In.ProjectEKA.HipServiceTest.Link.Builder
{
    using Bogus;
    using HipLibrary.Patient.Model;

    public static class TestBuilder
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