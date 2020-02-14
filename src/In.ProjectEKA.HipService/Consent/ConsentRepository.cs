namespace In.ProjectEKA.HipService.Consent
{
    using System.Threading.Tasks;
    using Database;
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
    }
}