namespace In.ProjectEKA.HipService.Consent
{
    using System.Threading.Tasks;
    using Common.Model;
    using Model;

    public interface IConsentRepository
    {
        public Task AddAsync(Consent consent);

        public Task UpdateAsync(string consentArtefactId, ConsentStatus status);

        public Task<Consent> GetFor(string consentArtefactId);
    }
}