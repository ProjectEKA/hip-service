namespace In.ProjectEKA.HipServiceTest.DataFlow.Builder
{
    using Bogus;
    using In.ProjectEKA.HipService.Common.Model;
    using In.ProjectEKA.HipService.DataFlow;
    using In.ProjectEKA.HipServiceTest.Common.Builder;
    using Consent = In.ProjectEKA.HipService.Consent.Model.Consent;
    using DataRequestLib = HipLibrary.Patient.Model.DataRequest;
    using GrantedContextLib = HipLibrary.Patient.Model.GrantedContext;
    using HiDataRangeLib = HipLibrary.Patient.Model.HiDataRange;
    using HiTypeLib = HipLibrary.Patient.Model.HiType;
    using KeyMaterialLib = HipLibrary.Patient.Model.KeyMaterial;
    using KeyStructureLib = HipLibrary.Patient.Model.KeyStructure;
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
        
        internal static DataRequestLib DataFlowRequest()
        {
            var faker = new Faker();
            var dataRequest = new DataRequestLib(new[]
                {
                    new GrantedContextLib(faker.Random.Hash(),
                        faker.Random.Hash())
                },
                new HiDataRangeLib(faker.Random.Word(),
                    faker.Random.Word()),
                "http://localhost:8003",
                new[] {HiTypeLib.Condition},
                faker.Random.Hash(),
                new KeyMaterialLib(faker.Random.Word(), faker.Random.Word(),
                    new KeyStructureLib(faker.Random.Word(), faker.Random.Word(),
                        faker.Random.Hash()), faker.Random.Word()));
            return dataRequest;
        }
    }
}