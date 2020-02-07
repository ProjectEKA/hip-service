namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Threading.Tasks;
    using Database;
    using Model;
    using Optional;
    using Logger;
    
    public class DataFlowRepository : IDataFlowRepository
    {
        private readonly DataFlowContext dataFlowContext;

        public DataFlowRepository(DataFlowContext dataFlowContext)
        {
            this.dataFlowContext = dataFlowContext;
        }

        public async Task<Option<Exception>> SaveRequestFor(
            string transactionId,
            HealthInformationRequest request)
        {
            var dataFlowRequest = new DataFlowRequest(transactionId, request);
            try
            {
                dataFlowContext.DataFlowRequest.Add(dataFlowRequest);
                await dataFlowContext.SaveChangesAsync();
                return Option.None<Exception>();
            }
            catch (Exception exception)
            {
                Log.Fatal(exception,"Error Occured");
                return Option.Some(exception);
            }
        }
    }
}