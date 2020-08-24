namespace In.ProjectEKA.HipService.DataFlow
{
    public class HealthInformationRequest
    {
        public HealthInformationRequest(string transactionId,
            Consent consent,
            DateRange dateRange,
            string dataPushUrl,
            KeyMaterial keyMaterial)
        {
            TransactionId = transactionId;
            Consent = consent;
            DateRange = dateRange;
            DataPushUrl = dataPushUrl;
            KeyMaterial = keyMaterial;
        }

        public string TransactionId { get; }

        public Consent Consent { get; }

        public DateRange DateRange { get; }

        public string DataPushUrl { get; }

        public KeyMaterial KeyMaterial { get; }
    }
}