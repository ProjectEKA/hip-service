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
            var entries = Option.Some(new List<Entry>
                {
                    new Entry(content, "application/json", checksum, null)
                }
                .AsEnumerable());

            collect.Setup(c => c.CollectData(dataRequest)).ReturnsAsync(data);
            dataEntryFactory.Setup(c => c.Process(data)).Returns(entries);
            dataFlowClient.Setup(c => c.SendDataToHiu(dataRequest, entries)).Verifiable();

            dataFlowMessageHandler.HandleDataFlowMessage(dataRequest);
            
            collect.VerifyAll();
            dataEntryFactory.VerifyAll();
            dataFlowClient.VerifyAll();
        }
    }
}