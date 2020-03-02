namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Threading.Tasks;
    using HipLibrary.Patient.Model;

    public interface IDataFlow
    {
        Task<Tuple<HealthInformationTransactionResponse, ErrorRepresentation>> HealthInformationRequestFor(
            HealthInformationRequest request);

        Task<Tuple<HealthInformationResponse, ErrorRepresentation>> HealthInformationFor(
            string informationId,
            string token,
            string transactionId);
    }
}