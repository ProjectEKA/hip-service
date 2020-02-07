
namespace In.ProjectEKA.HipService.DataFlow
{
    using System.Collections.Generic;

    public class DataFlowArtefact
    {
        public IEnumerable<GrantedContext> CareContexts { get; }
        public HiDataRange DataRange { get; }
        public string CallBackUrl { get; }
        public IEnumerable<HiType> HiType { get; }

        public DataFlowArtefact(IEnumerable<GrantedContext> careContexts,
                                HiDataRange dataRange, 
                                string callBackUrl, 
                                IEnumerable<HiType> hiType)
        {
            CareContexts = careContexts;
            DataRange = dataRange;
            CallBackUrl = callBackUrl;
            HiType = hiType;
        }
    }
}