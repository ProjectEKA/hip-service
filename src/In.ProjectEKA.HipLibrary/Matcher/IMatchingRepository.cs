namespace In.ProjectEKA.HipLibrary.Matcher
{
    using System.Linq;
    using System.Threading.Tasks;
    using Patient.Model;

    public interface IMatchingRepository
    {
        Task<IQueryable<Patient>> Where(DiscoveryRequest request);
    }
}