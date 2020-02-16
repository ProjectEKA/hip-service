using System.Collections.Generic;
using In.ProjectEKA.HipLibrary.Patient.Model;

namespace In.ProjectEKA.DefaultHipTest.DataFlow.Builder
{
    internal class DataRequestBuilder
    {
        public IEnumerable<GrantedContext> CareContexts;
        public HiDataRange DataRange;
        public string CallBackUrl;
        public IEnumerable<HiType> HiType;
        public string TransactionId;
        
        public DataRequest Build()
        {
            return new DataRequest(CareContexts, DataRange, CallBackUrl, HiType, TransactionId);
        }
    }
}