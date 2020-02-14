namespace In.ProjectEKA.HipService.DataFlow
{
    using Optional;
    using System;
    using System.Threading.Tasks;
    using Common.Model;

    public interface IDataFlowRepository
    {
        Task<Option<Exception>> SaveRequestFor(string transactionId, HealthInformationRequest request);
        public Tuple<ConsentArtefact, Exception> GetFor(string consentId);
    }
}