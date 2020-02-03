namespace In.ProjectEKA.HipService.Link
{
    using System.Threading.Tasks;

    public interface IDiscoveryRequestRepository
    {
        Task Delete(string transactionId, string consentManagerUserId);

        Task<bool> RequestExistsFor(string transactionId);
    }
}