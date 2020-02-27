namespace In.ProjectEKA.HipService.DataFlow
{
    using HipLibrary.Patient;
    using Task = System.Threading.Tasks.Task;

    public class DataFlowMessageHandler
    {
        private readonly ICollect collect;
        private readonly IDataFlowClient dataFlowClient;
        private readonly IDataEntryFactory dataEntryFactory;

        public DataFlowMessageHandler(
            ICollect collect,
            IDataFlowClient dataFlowClient, IDataEntryFactory dataEntryFactory)
        {
            this.collect = collect;
            this.dataFlowClient = dataFlowClient;
            this.dataEntryFactory = dataEntryFactory;
        }

        public async Task HandleDataFlowMessage(HipLibrary.Patient.Model.DataRequest dataRequest)
        {
            var data = await collect.CollectData(dataRequest);
            var entries = dataEntryFactory.Process(data);
            dataFlowClient.SendDataToHiu(dataRequest, entries);
        }
    }
}