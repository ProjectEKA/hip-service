namespace In.ProjectEKA.HipServiceTest.DataFlow.Builder
{
    using System;
    using System.Collections.Generic;
    using Bogus;
    using HipService.DataFlow.Model;
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
    using GrantedContext = HipLibrary.Patient.Model.GrantedContext;
    using HiType = HipLibrary.Patient.Model.HiType;

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
                new KeyMaterial(faker.Random.Word(), faker.Random.Word(),
                    new KeyStructure("", "", faker.Random.Hash()),
                    faker.Random.Hash()));
        }

        internal static Faker<ConsentArtefactBuilder> ConsentArtefact()
        {
            return new Faker<ConsentArtefactBuilder>();
        }

        internal static HipLibrary.Patient.Model.DataRequest DataRequest(string transactionId)
        {
            var grantedContexts = new List<GrantedContext>();
            var hiDataRange = new HipLibrary.Patient.Model.HiDataRange("from", "to");
            var callBackUrl = "http://callback";
            var hiTypes = new List<HiType>();
            var keyMaterial = new KeyMaterialLib(faker.Random.Word(), faker.Random.Word(),
                new KeyStructureLib("", "", faker.Random.Hash()),
                faker.Random.Hash());
            return new HipLibrary.Patient.Model.DataRequest(grantedContexts, hiDataRange, callBackUrl, hiTypes,
                transactionId, keyMaterial);
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
            var fakerDataRequest = new Faker();
            var dataRequest = new DataRequestLib(new[]
                {
                    new GrantedContextLib(fakerDataRequest.Random.Hash(),
                        fakerDataRequest.Random.Hash())
                },
                new HiDataRangeLib(fakerDataRequest.Random.Word(),
                    fakerDataRequest.Random.Word()),
                "http://localhost:8003",
                new[] {HiTypeLib.Condition},
                fakerDataRequest.Random.Hash(),
                new KeyMaterialLib(fakerDataRequest.Random.Word(), fakerDataRequest.Random.Word(),
                    new KeyStructureLib(fakerDataRequest.Random.Word(), fakerDataRequest.Random.Word(),
                        fakerDataRequest.Random.Hash()), fakerDataRequest.Random.Word()));
            return dataRequest;
        }

        internal static Entry Entry()
        {
            var content = Faker().Random.String();
            var media = Faker().Random.String();
            var checksum = Faker().Random.Hash();
            return new Entry(content, media, checksum, null);
        }
        internal static HealthInformation HealthInformation(string token, DateTime dateTime)
        {
            var linkId = Faker().Random.Hash();
            return new HealthInformation(linkId, Entry(), dateTime, token);
        }

        internal static HealthInformationResponse HealthInformationResponse(string transactionId)
        {
            return new HealthInformationResponse(transactionId, Entry());
        }
    }
}