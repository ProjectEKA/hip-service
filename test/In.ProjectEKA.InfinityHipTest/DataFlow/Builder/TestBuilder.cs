namespace In.ProjectEKA.HipServiceTest.DataFlow.Builder
{
    using System.Collections.Generic;
    using Bogus;
    using In.ProjectEKA.HipLibrary.Patient.Model;
    using KeyMaterialLib = HipLibrary.Patient.Model.KeyMaterial;
    using KeyStructureLib = HipLibrary.Patient.Model.KeyStructure;

    public static class TestBuilder
    {
        private static Faker faker;

        internal static Faker Faker()
        {
            return faker ??= new Faker();
        }

        internal static HipLibrary.Patient.Model.DataRequest DataRequest(string transactionId)
        {
            var grantedContexts = new List<GrantedContext>();
            var hiDataRange = new HipLibrary.Patient.Model.HiDataRange("from", "to");
            var callBackUrl = "http://callback";
            var hiTypes = new List<HiType>();
            hiTypes.Add(HiType.Observation);
            var keyMaterial = new KeyMaterialLib(faker.Random.Word(), faker.Random.Word(),
                new KeyStructureLib("", "", faker.Random.Hash()),
                faker.Random.Hash());
            return new HipLibrary.Patient.Model.DataRequest(grantedContexts, hiDataRange, callBackUrl, hiTypes,
                transactionId, keyMaterial);
        }
    }
}