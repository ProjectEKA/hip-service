using System.Threading.Tasks;
using hip_service.Discovery.Patient.Model;

namespace hip_service.Discovery.Patient
{
    public interface IDiscoveryRequestRepository
    {
        Task Add(DiscoveryRequest discoveryRequest);
        Task Delete(string transactionId, string consentManagerUserId);
    }
}