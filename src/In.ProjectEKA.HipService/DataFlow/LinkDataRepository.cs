namespace In.ProjectEKA.HipService.DataFlow
{
    using System.Threading.Tasks;
    using Database;
    using Microsoft.EntityFrameworkCore;
    using Model;

    public class LinkDataRepository: ILinkDataRepository
    {
        private readonly DataFlowContext dataFlowContext;

        public LinkDataRepository(DataFlowContext dataFlowContext)
        {
            this.dataFlowContext = dataFlowContext;
        }

        public void Add(LinkData linkData)
        {
            dataFlowContext.LinkData.Add(linkData);
            dataFlowContext.SaveChanges();
        }

        public async Task<LinkData> GetAsync(string linkId)
        {
            return await dataFlowContext.LinkData.FirstOrDefaultAsync(data => data.LinkId == linkId);
        }
    }
}