using Bogus;
using HipLibrary.Patient.Model.Request;
using In.ProjectEKA.DefaultHip.Link;
using In.ProjectEKA.HipService.Link;
using Microsoft.Extensions.Configuration;
using PatientLinkReferenceRequest = In.ProjectEKA.HipService.Link.PatientLinkReferenceRequest;

namespace In.ProjectEKA.HipServiceTest.Link.Builder
{
    using Bogus;

    public static class TestBuilder
    {
        private static Faker faker;
        
        internal static Faker Faker() => faker ??= new Faker();

        internal static PatientLinkReferenceRequest GetFakeLinkRequest()
        {
            return new PatientLinkReferenceRequest(faker.Random.Hash()
                , new LinkReference(faker.Random.Hash()
                    , referenceNumber:faker.Random.Hash()
                    , new [] {new CareContext(faker.Random.Hash())}));
        }
    }
}