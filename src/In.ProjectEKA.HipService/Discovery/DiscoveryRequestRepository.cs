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
            discoveryContext.DiscoveryRequest.Add(discoveryRequest);
            await discoveryContext.SaveChangesAsync();
        }

        public async Task Delete(string transactionId, string consentManagerUserId)
        {
            var discoveryRequest = await discoveryContext.DiscoveryRequest
                .FirstAsync(request =>
                    request.TransactionId == transactionId &&
                    request.ConsentManagerUserId == consentManagerUserId);
            discoveryContext.Remove(discoveryRequest);
            await discoveryContext.SaveChangesAsync();
        }

        public Task<bool> RequestExistsFor(string transactionId, string consentManagerUserId)
        {
            return discoveryContext.DiscoveryRequest
                .AnyAsync(request => request.TransactionId == transactionId &&
                                     request.ConsentManagerUserId == consentManagerUserId);
        }

        public Task<bool> RequestExistsFor(string transactionId)
        {
            return discoveryContext.DiscoveryRequest
                .AnyAsync(request => request.TransactionId == transactionId);
        }
    }
}