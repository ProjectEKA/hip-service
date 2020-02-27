namespace In.ProjectEKA.HipService.DataFlow
{
    using HipLibrary.Patient;
    using Task = System.Threading.Tasks.Task;

    public class DataFlowMessageHandler
    {
        private readonly ICollect collect;
        private readonly DataFlowClient dataFlowClient;
        private readonly DataEntryFactory dataEntryFactory;

        public DataFlowMessageHandler(
            ICollect collect,
            DataFlowClient dataFlowClient,
            DataEntryFactory dataEntryFactory)
        {
            this.collect = collect;
            this.dataFlowClient = dataFlowClient;
            this.dataEntryFactory = dataEntryFactory;
        }

        public async Task HandleDataFlowMessage(HipLibrary.Patient.Model.DataRequest dataRequest)
        {
            var data = await collect.CollectData(dataRequest);
            var entries = dataEntryFactory.Process(data);
            // Create Key Material
            dataFlowClient.SendDataToHiu(dataRequest, entries, null);
        }
    }
}