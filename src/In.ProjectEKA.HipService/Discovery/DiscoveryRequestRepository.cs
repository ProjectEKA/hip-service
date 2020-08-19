namespace In.ProjectEKA.HipService.Discovery
{
    using System.Threading.Tasks;
    using Database;
    using Microsoft.EntityFrameworkCore;
    using Model;

    public class DiscoveryRequestRepository : IDiscoveryRequestRepository
    {
        private readonly DiscoveryContext discoveryContext;

        public DiscoveryRequestRepository(DiscoveryContext discoveryContext)
        {
            this.discoveryContext = discoveryContext;
        }

        public async Task Add(DiscoveryRequest discoveryRequest)
        {
            await discoveryContext.DiscoveryRequest.AddAsync(discoveryRequest);
            await discoveryContext.SaveChangesAsync();
        }

        public async Task Delete(string requestId, string consentManagerUserId)
        {
            var discoveryRequest = await discoveryContext.DiscoveryRequest
                .FirstAsync(request =>
                    request.TransactionId == requestId &&
                    request.ConsentManagerUserId == consentManagerUserId);
            discoveryContext.Remove(discoveryRequest);
            await discoveryContext.SaveChangesAsync();
        }

        public Task<bool> RequestExistsFor(
            string requestId,
            string consentManagerUserId,
            string patientReferenceNumber)
        {
            return discoveryContext.DiscoveryRequest
                .AnyAsync(request => request.TransactionId == requestId
                                     && request.ConsentManagerUserId == consentManagerUserId
                                     && request.PatientReferenceNumber == patientReferenceNumber);
        }

        public Task<bool> RequestExistsFor(string requestId)
        {
            return discoveryContext.DiscoveryRequest
                .AnyAsync(request => request.TransactionId == requestId);
        }
    }
}