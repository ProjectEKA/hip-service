using System.Collections.Generic;
using In.ProjectEKA.HipLibrary.Patient.Model;

namespace In.ProjectEKA.HipServiceTest.DataFlow.Builder
{
    internal class DataRequestBuilder
    {
        private IEnumerable<GrantedContext> CareContexts;
        private HiDataRange DataRange;
        private string CallBackUrl;
        private IEnumerable<HiType> HiType;
        private string TransactionId;
        
        public DataRequest Build()
        {
            return new DataRequest(CareContexts, DataRange, CallBackUrl, HiType, TransactionId);
        }
    }
}