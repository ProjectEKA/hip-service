namespace In.ProjectEKA.HipService.DataFlow
{
    using System.Threading.Tasks;
    using Database;
    using Microsoft.EntityFrameworkCore;
    using Model;

    public class HealthInformationRepository: IHealthInformationRepository
    {
        private readonly DataFlowContext dataFlowContext;

        public HealthInformationRepository(DataFlowContext dataFlowContext)
        {
            this.dataFlowContext = dataFlowContext;
        }

        public void Add(HealthInformation healthInformation)
        {
            dataFlowContext.HealthInformation.Add(healthInformation);
            dataFlowContext.SaveChanges();
        }

        public async Task<HealthInformation> GetAsync(string linkId)
        {
            return await dataFlowContext.HealthInformation.FirstOrDefaultAsync(data => data.InformationId == linkId);
        }
    }
}