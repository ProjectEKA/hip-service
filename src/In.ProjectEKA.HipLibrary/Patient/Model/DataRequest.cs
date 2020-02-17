namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    using System.Collections.Generic;

    public class DataRequest
    {
        public DataRequest(IEnumerable<GrantedContext> careContexts,
            HiDataRange dataRange,
            string callBackUrl,
            IEnumerable<HiType> hiType,
            string transactionId)
        {
            CareContexts = careContexts;
            DataRange = dataRange;
            CallBackUrl = callBackUrl;
            HiType = hiType;
            TransactionId = transactionId;
        }

        public IEnumerable<GrantedContext> CareContexts { get; }
        public HiDataRange DataRange { get; }
        public string CallBackUrl { get; }
        public IEnumerable<HiType> HiType { get; }
        public string TransactionId { get; }
    }
}