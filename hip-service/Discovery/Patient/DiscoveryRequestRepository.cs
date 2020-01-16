using System.Threading.Tasks;
using hip_service.Discovery.Patient.Database;
using hip_service.Discovery.Patient.Model;

namespace hip_service.Discovery.Patient
{
    public class DiscoveryRequestRepository: IDiscoveryRequestRepository
    {
        private readonly DiscoveryContext discoveryContext;

        public DiscoveryRequestRepository(DiscoveryContext discoveryContext)
        {
            this.discoveryContext = discoveryContext;
        }

        public async Task Add(DiscoveryRequest discoveryRequest)
        {
            discoveryContext.DiscoveryRequest.Add(discoveryRequest);
            await discoveryContext.SaveChangesAsync();
        }
    }
}