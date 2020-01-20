namespace In.ProjectEKA.DefaultHip.Discovery
{
    using System.Threading.Tasks;
    using Model;

    public interface IDiscoveryRequestRepository
    {
        Task Add(DiscoveryRequest discoveryRequest);

        Task Delete(string transactionId, string consentManagerUserId);
    }
}