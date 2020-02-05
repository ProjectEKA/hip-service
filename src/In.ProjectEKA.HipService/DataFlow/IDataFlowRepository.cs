namespace In.ProjectEKA.HipService.DataFlow
{
    using Optional;
    using System;
    using System.Threading.Tasks;

    public interface IDataFlowRepository
    {
        Task<Option<Exception>> SaveRequestFor(string transactionId, HealthInformationRequest request);
    }
}