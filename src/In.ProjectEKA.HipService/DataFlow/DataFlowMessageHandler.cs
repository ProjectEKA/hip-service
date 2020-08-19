namespace In.ProjectEKA.HipService.DataFlow
{
    using System.Threading.Tasks;
    using HipLibrary.Patient;

    public class DataFlowMessageHandler
    {
        private readonly ICollect collect;
        private readonly DataEntryFactory dataEntryFactory;
        private readonly DataFlowClient dataFlowClient;

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
            var sentKeyMaterial = dataRequest.KeyMaterial;
            var data = await collect.CollectData(dataRequest).ConfigureAwait(false);
            var encryptedEntries = data.FlatMap(entries =>
                dataEntryFactory.Process(entries, sentKeyMaterial, dataRequest.TransactionId));
            encryptedEntries.MatchSome(async entries =>
                await dataFlowClient.SendDataToHiu(dataRequest,
                    entries.Entries,
                    entries.KeyMaterial).ConfigureAwait(false));
        }
    }
}