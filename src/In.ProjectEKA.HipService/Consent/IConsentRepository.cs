
namespace In.ProjectEKA.HipService.Consent
{
    using System.Threading.Tasks;
    using In.ProjectEKA.HipService.Common.Model;

    public interface IConsentRepository
    {
        public Task AddAsync(Model.Consent consent);

        public Task UpdateAsync(string consentArtefactId, ConsentStatus status);

        public Task<Model.Consent> GetFor(string consentArtefactId);
    }
}