namespace In.ProjectEKA.HipServiceTest.DataFlow
{
    using System.Collections.Generic;
    using System.Linq;
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
            var dataFlowClient = new Mock<DataFlowClient>();
            var dataEntryFactory = new Mock<DataEntryFactory>();
            var dataFlowMessageHandler =
                new DataFlowMessageHandler(collect.Object, dataFlowClient.Object, dataEntryFactory.Object);
            var dataRequest = TestBuilder.DataRequest(TestBuilder.Faker().Random.Hash());
            var data = Option.Some(new Entries(new List<Bundle> {new Bundle()}));
            var content = TestBuilder.Faker().Random.String();
            var checksum = TestBuilder.Faker().Random.Hash();
            var entries = new List<Entry>
            {
                new Entry(content, "application/json", checksum, null)
            };
            var requestKeyMaterial = TestBuilder.KeyMaterialLib();
            collect.Setup(c => c.CollectData(dataRequest)).ReturnsAsync(data);
            var keyMaterial = TestBuilder.KeyMaterial();
            var encryptedEntriesValue = new EncryptedEntries(entries.AsEnumerable(), keyMaterial);
            var encryptedEntries = Option.Some(encryptedEntriesValue);
            dataEntryFactory.Setup(e => e.Process(data, requestKeyMaterial))
                .Returns(encryptedEntries);
            dataFlowClient.Setup(c => c.SendDataToHiu(dataRequest,
                encryptedEntriesValue.Entries, encryptedEntriesValue.KeyMaterial)).Verifiable();

            dataFlowMessageHandler.HandleDataFlowMessage(dataRequest);
            collect.VerifyAll();
            dataEntryFactory.Verify();
        }
    }
}