namespace In.ProjectEKA.HipService.Discovery
{
    using System.Linq;
    using System.Threading.Tasks;
    using Model;

    public interface IMatchingRepository
    {
        Task<IQueryable<Patient>> Where(HipLibrary.Patient.Model.Request.DiscoveryRequest request);
    }
}