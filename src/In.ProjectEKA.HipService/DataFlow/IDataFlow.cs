namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Threading.Tasks;
    using HipLibrary.Patient.Model;
    
    public interface IDataFlow
    {
        Task<Tuple<HealthInformationResponse, ErrorRepresentation>> HealthInformationRequestFor(
            HealthInformationRequest request);
    }
}