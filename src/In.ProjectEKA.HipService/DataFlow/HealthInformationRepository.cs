namespace In.ProjectEKA.HipService.DataFlow
{
    using System.Threading.Tasks;
    using Database;
    using Microsoft.EntityFrameworkCore;
    using Model;
    using Optional;

    public class HealthInformationRepository : IHealthInformationRepository
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

        public async Task<Option<HealthInformation>> GetAsync(string informationId)
        {
            var healthInformation = await dataFlowContext.HealthInformation
                .FirstOrDefaultAsync(data => data.InformationId == informationId);
            return healthInformation != null ? Option.Some(healthInformation) : Option.None<HealthInformation>();
        }
    }
}