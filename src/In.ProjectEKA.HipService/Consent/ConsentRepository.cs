namespace In.ProjectEKA.HipService.Consent
{
    using System.Threading.Tasks;
    using Database;
    using Microsoft.EntityFrameworkCore;
    using Model;

    public class ConsentRepository: IConsentRepository
    {
        private readonly ConsentContext consentContext;

        public ConsentRepository(ConsentContext consentContext)
        {
            this.consentContext = consentContext;
        }

        public async Task AddAsync(Consent consent)
        {
            await consentContext.ConsentArtefact.AddAsync(consent); 
            await consentContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Consent consent)
        {
            var consentArtefact = await consentContext.ConsentArtefact
                .FirstOrDefaultAsync(c => c.ConsentArtefactId == consent.ConsentArtefactId);
            consentArtefact.Status = consent.Status;
            await consentContext.SaveChangesAsync();
        }

        public async Task<Consent> GetFor(string consentArtefactId)
        {
            return await consentContext.ConsentArtefact
                .FirstOrDefaultAsync(x => x.ConsentArtefactId == consentArtefactId);
        }
    }
}