namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Threading.Tasks;
    using HipLibrary.Patient.Model.Response;
    
    public interface IDataFlow
    {
        Task<Tuple<HealthInformationResponse, ErrorResponse>> HealthInformationRequestFor(
            HealthInformationRequest request);
    }
}