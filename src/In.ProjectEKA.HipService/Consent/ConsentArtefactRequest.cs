namespace In.ProjectEKA.HipService.Consent
{
    using Common.Model;

    public class ConsentArtefactRequest
    {
        public ConsentArtefactRequest(string signature, ConsentArtefact consentDetail, ConsentStatus status,
            string consentId)
        {
            Signature = signature;
            ConsentDetail = consentDetail;
            Status = status;
            ConsentId = consentId;
        }

        public string Signature { get; set; }
        public ConsentArtefact ConsentDetail { get; set; }
        public ConsentStatus Status { get; set; }
        public string ConsentId { get; set; }
    }
}