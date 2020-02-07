
namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Threading.Tasks;
    using In.ProjectEKA.HipLibrary.Patient.Model;
    
    public interface IDataFlowArtefactRepository
    {
        Task<Tuple<DataFlowArtefact, ErrorRepresentation>> GetFor(HealthInformationRequest healthInformationRequest);
    }
}