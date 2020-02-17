namespace In.ProjectEKA.HipService.Consent
{
    using Common.Model;

    public class ConsentArtefactRequest
    {
        public string Signature { get; set; }
        public ConsentArtefact ConsentDetail { get; set; }
        public ConsentStatus Status { get; set; }

        public ConsentArtefactRequest(string signature, ConsentArtefact consentDetail, ConsentStatus status)
        {
            Signature = signature;
            ConsentDetail = consentDetail;
            Status = status;
        }
    }
}