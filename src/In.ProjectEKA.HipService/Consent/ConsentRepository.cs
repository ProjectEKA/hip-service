
namespace In.ProjectEKA.HipService.Consent
{
    using System.Threading.Tasks;
    using In.ProjectEKA.HipService.Common.Model;
    using In.ProjectEKA.HipService.Consent.Database;
    using Microsoft.EntityFrameworkCore;

    public class ConsentRepository : IConsentRepository
    {
        private readonly ConsentContext consentContext;

        public ConsentRepository(ConsentContext consentContext)
        {
            this.consentContext = consentContext;
        }

        public async Task AddAsync(Model.Consent consent)
        {
            await consentContext.ConsentArtefact.AddAsync(consent);
            await consentContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(string consentArtefactId, ConsentStatus status)
        {
            var consentArtefact = await consentContext.ConsentArtefact
                .FirstOrDefaultAsync(c => c.ConsentArtefactId == consentArtefactId);
            consentArtefact.Status = status;
            await consentContext.SaveChangesAsync();
        }

        public async Task<Model.Consent> GetFor(string consentArtefactId)
        {
            return await consentContext.ConsentArtefact
                .FirstOrDefaultAsync(x => x.ConsentArtefactId == consentArtefactId);
        }
    }
}