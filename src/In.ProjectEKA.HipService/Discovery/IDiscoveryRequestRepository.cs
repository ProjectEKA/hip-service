namespace In.ProjectEKA.HipService.Discovery
{
    using System.Threading.Tasks;
    using Model;

    public interface IDiscoveryRequestRepository
    {
        Task Add(DiscoveryRequest discoveryRequest);

        Task Delete(string transactionId, string consentManagerUserId);

        Task<bool> RequestExistsFor(string transactionId, string consentManagerUserId);

        Task<bool> RequestExistsFor(string transactionId);
    }
}