namespace In.ProjectEKA.HipService.DataFlow
{
    using System.Collections.Generic;
    using Common.Model;

    public class DataRequest
    {
        public IEnumerable<GrantedContext> CareContexts { get; }
        public HiDataRange DataRange { get; }
        public string DataPushUrl { get; }
        public IEnumerable<HiType> HiType { get; }
        public string TransactionId { get; }
        public KeyMaterial KeyMaterial { get; }

        public string ConsentManagerId { get; }

        public DataRequest(IEnumerable<GrantedContext> careContexts,
            HiDataRange dataRange,
            string dataPushUrl,
            IEnumerable<HiType> hiType,
            string transactionId,
            KeyMaterial keyMaterial,
            string consentManagerId)
        {
            CareContexts = careContexts;
            DataRange = dataRange;
            DataPushUrl = dataPushUrl;
            HiType = hiType;
            TransactionId = transactionId;
            KeyMaterial = keyMaterial;
            ConsentManagerId = consentManagerId;
        }
    }
}