using System.Threading.Tasks;
using In.ProjectEKA.HipService.Discovery.Patient.Model;

namespace In.ProjectEKA.HipService.Discovery.Patient
{
    public interface IDiscoveryRequestRepository
    {
        Task Add(DiscoveryRequest discoveryRequest);
        Task Delete(string transactionId, string consentManagerUserId);
    }
}