namespace In.ProjectEKA.HipService.DataFlow
{
    public class HIRequest
    {
        public HIRequest(Consent consent, DateRange dateRange, string dataPushUrl, KeyMaterial keyMaterial)
        {
            Consent = consent;
            DateRange = dateRange;
            DataPushUrl = dataPushUrl;
            KeyMaterial = keyMaterial;
        }

        public Consent Consent { get; }
        public DateRange DateRange { get; }
        public string DataPushUrl { get; }
        public KeyMaterial KeyMaterial { get; }
    }
}