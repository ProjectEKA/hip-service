
namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Threading.Tasks;
    using In.ProjectEKA.HipLibrary.Patient.Model;
    
    public interface IDataFlowArtefactRepository
    {
        Tuple<DataFlowArtefact, ErrorRepresentation> GetFor(HealthInformationRequest healthInformationRequest);
    }
}