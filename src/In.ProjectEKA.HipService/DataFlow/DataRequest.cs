namespace In.ProjectEKA.HipService.DataFlow
{
    using System.Collections.Generic;
    using Common.Model;

    public class DataRequest
    {
        public DataRequest(IEnumerable<GrantedContext> careContexts,
            DateRange dateRange,
            string dataPushUrl,
            IEnumerable<HiType> hiType,
            string transactionId,
            KeyMaterial keyMaterial,
            string gatewayId,
            string consentId,
            string cmSuffix,
            string correlationId)
        {
            CareContexts = careContexts;
            DateRange = dateRange;
            DataPushUrl = dataPushUrl;
            HiType = hiType;
            TransactionId = transactionId;
            KeyMaterial = keyMaterial;
            GatewayId = gatewayId;
            ConsentId = consentId;
            CmSuffix = cmSuffix;
            CorrelationId = correlationId;
        }

        public IEnumerable<GrantedContext> CareContexts { get; }
        public DateRange DateRange { get; }
        public string DataPushUrl { get; }
        public IEnumerable<HiType> HiType { get; }
        public string TransactionId { get; }
        public KeyMaterial KeyMaterial { get; }
        public string GatewayId { get; }
        public string ConsentId { get; }
        public string CmSuffix { get; }
        public string CorrelationId { get; }
    }
}