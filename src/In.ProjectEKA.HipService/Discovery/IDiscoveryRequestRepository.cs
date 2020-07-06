namespace In.ProjectEKA.HipService.Discovery
{
    using System.Threading.Tasks;
    using Model;

    public interface IDiscoveryRequestRepository
    {
        Task Add(DiscoveryRequest discoveryRequest);

        Task Delete(string requestId, string consentManagerUserId);

        Task<bool> RequestExistsFor(string requestId, string consentManagerUserId, string patientReferenceNumber);

        Task<bool> RequestExistsFor(string requestId);
        
        Task<DiscoveryRequest> GetRequestFor(string requestTransactionId);
    }
}