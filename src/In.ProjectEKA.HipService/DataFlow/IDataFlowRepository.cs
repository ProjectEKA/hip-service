namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Threading.Tasks;
    using Optional;

    public interface IDataFlowRepository
    {
        Task<Option<Exception>> SaveRequest(string transactionId, HealthInformationRequest request);
    }
}