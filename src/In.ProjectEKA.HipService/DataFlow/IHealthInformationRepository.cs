namespace In.ProjectEKA.HipService.DataFlow
{
    using System.Threading.Tasks;
    using Model;

    public interface IHealthInformationRepository
    {
        void Add(HealthInformation healthInformation);

        Task<HealthInformation> GetAsync(string linkId);
    }
}