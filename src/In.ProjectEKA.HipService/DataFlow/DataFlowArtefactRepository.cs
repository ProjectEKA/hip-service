
namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using In.ProjectEKA.HipLibrary.Patient.Model;
    
    public class DataFlowArtefactRepository : IDataFlowArtefactRepository
    {
        public Tuple<DataFlowArtefact, ErrorRepresentation> GetFor(
            HealthInformationRequest healthInformationRequest)
        {
            var dataFlowMessage = new DataFlowArtefact(
                new List<GrantedContext>{new GrantedContext("5",
                    "130")},
                healthInformationRequest.HiDataRange,
                healthInformationRequest.CallBackUrl,
                new List<HiType>{HiType.Condition});
            return new Tuple<DataFlowArtefact, ErrorRepresentation>(dataFlowMessage, null);
        }
    }
}