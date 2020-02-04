namespace In.ProjectEKA.HipLibrary.Patient
{
    using System.Linq;
    using System.Threading.Tasks;
    using Model;

    public interface IMatchingRepository
    {
        Task<IQueryable<Patient>> Where(Model.Request.DiscoveryRequest request);
    }
}