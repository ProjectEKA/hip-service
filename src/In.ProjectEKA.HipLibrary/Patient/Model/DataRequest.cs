namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    using System.Collections.Generic;
    using System.Linq;

    public class DataRequest
    {
        public DataRequest(IEnumerable<GrantedContext> careContexts,
            DateRange dateRange,
            string dataPushUrl,
            IEnumerable<HiType> hiType,
            string transactionId,
            KeyMaterial keyMaterial,
            string consentManagerId)
        {
            CareContexts = careContexts;
            DateRange = dateRange;
            DataPushUrl = dataPushUrl;
            HiType = hiType;
            TransactionId = transactionId;
            KeyMaterial = keyMaterial;
            ConsentManagerId = consentManagerId;
        }

        public IEnumerable<GrantedContext> CareContexts { get; }
        public DateRange DateRange { get; }
        public string DataPushUrl { get; }
        public IEnumerable<HiType> HiType { get; }
        public string TransactionId { get; }
        public KeyMaterial KeyMaterial { get; }
        public string ConsentManagerId { get; }


        public override string ToString()
        {
            var hiTypes = HiType
                .Select(hiType => hiType.ToString())
                .Aggregate("", (source, value) => source + " " + value);
            return $"Data Request with {hiTypes}";
        }
    }
}