namespace In.ProjectEKA.HipServiceTest.DataFlow.Builder
{
    using System.Collections.Generic;
    using HipLibrary.Patient.Model;

    internal class DataRequestBuilder
    {
        private string CallBackUrl;
        private IEnumerable<GrantedContext> CareContexts;
        private HiDataRange DataRange;
        private IEnumerable<HiType> HiType;
        private string TransactionId;
        private KeyMaterial KeyMaterial;

        public DataRequest Build()
        {
            return new DataRequest(CareContexts, DataRange, CallBackUrl, HiType, TransactionId, KeyMaterial);
        }
    }
}