namespace In.ProjectEKA.HipService.DataFlow
{
    using System.Collections.Generic;
    using Common.Model;

    public class DataRequest
    {
        public IEnumerable<GrantedContext> CareContexts { get; }
        public DateRange DateRange { get; }
        public string DataPushUrl { get; }
        public IEnumerable<HiType> HiType { get; }
        public string RequestId { get; }
        public KeyMaterial KeyMaterial { get; }
        public string ConsentManagerId { get; }
        public string ConsentId { get; }

        public DataRequest(IEnumerable<GrantedContext> careContexts,
            DateRange dateRange,
            string dataPushUrl,
            IEnumerable<HiType> hiType,
            string transactionId,
            KeyMaterial keyMaterial,
            string consentManagerId,
            string consentId)
        {
            CareContexts = careContexts;
            DateRange = dateRange;
            DataPushUrl = dataPushUrl;
            HiType = hiType;
            RequestId = transactionId;
            KeyMaterial = keyMaterial;
            ConsentManagerId = consentManagerId;
            ConsentId = consentId;
        }
    }
}