namespace In.ProjectEKA.HipService.DataFlow
{
    using System.Threading.Tasks;
    using Model;
    using Optional;

    public interface IHealthInformationRepository
    {
        void Add(HealthInformation healthInformation);

        Task<Option<HealthInformation>> GetAsync(string informationId);
    }
}