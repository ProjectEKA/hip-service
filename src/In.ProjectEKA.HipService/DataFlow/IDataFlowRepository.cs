namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Threading.Tasks;
    using Optional;

    public interface IDataFlowRepository
    {
        Task<Option<Exception>> SaveRequestFor(string transactionId, HealthInformationRequest request);
    }
}