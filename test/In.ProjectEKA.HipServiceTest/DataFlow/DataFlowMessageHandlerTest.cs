namespace In.ProjectEKA.HipServiceTest.DataFlow
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mime;
    using Builder;
    using HipLibrary.Patient;
    using HipLibrary.Patient.Model;
    using HipService.DataFlow;
    using Hl7.Fhir.Model;
    using Moq;
    using Optional;
    using Xunit;

    public class DataFlowMessageHandlerTest
    {
        [Fact]
        private void ShouldProcessMessage()
        {
            var collect = new Mock<ICollect>();
            var dataFlowClient = new Mock<DataFlowClient>(MockBehavior.Strict, null, null, null);
            var dataEntryFactory = new Mock<DataEntryFactory>();
            var dataFlowMessageHandler =
                new DataFlowMessageHandler(collect.Object, dataFlowClient.Object, dataEntryFactory.Object);
            var transactionId = TestBuilder.Faker().Random.Uuid().ToString();
            var dataRequest = TestBuilder.DataRequest(transactionId);
            var careBundles = new List<CareBundle> {new CareBundle("careContextReference", new Bundle())};
            var entries = new Entries(careBundles);
            var data = Option.Some(entries);
            var content = TestBuilder.Faker().Random.String();
            var checksum = TestBuilder.Faker().Random.Hash();
            var entriesList = new List<Entry>
            {
                new Entry(content, MediaTypeNames.Application.Json, checksum, null, "careContextReference")
            };
            var requestKeyMaterial = TestBuilder.KeyMaterialLib();
            collect.Setup(c => c.CollectData(dataRequest)).ReturnsAsync(data);
            var keyMaterial = TestBuilder.KeyMaterial();
            var encryptedEntriesValue = new EncryptedEntries(entriesList.AsEnumerable(), keyMaterial);
            var encryptedEntries = Option.Some(encryptedEntriesValue);
            dataEntryFactory.Setup(e => e.Process(entries, requestKeyMaterial, transactionId))
                .Returns(encryptedEntries);
            dataFlowClient.Setup(c => c.SendDataToHiu(dataRequest,
                encryptedEntriesValue.Entries, encryptedEntriesValue.KeyMaterial)).Verifiable();

            dataFlowMessageHandler.HandleDataFlowMessage(dataRequest);

            collect.VerifyAll();
            dataEntryFactory.Verify();
        }
    }
}