namespace In.ProjectEKA.DefaultHipTest.DataFlow.Builder
{
    using System.Collections.Generic;
    using In.ProjectEKA.HipLibrary.Patient.Model;

    internal class DataRequestBuilder
    {
        public string CallBackUrl;
        public IEnumerable<GrantedContext> CareContexts;
        public HiDataRange DataRange;
        public IEnumerable<HiType> HiType;
        public string TransactionId;

        public DataRequest Build()
        {
            return new DataRequest(CareContexts, DataRange, CallBackUrl, HiType, TransactionId);
        }
    }
}