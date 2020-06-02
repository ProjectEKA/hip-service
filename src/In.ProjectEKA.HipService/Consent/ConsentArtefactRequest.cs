using In.ProjectEKA.HipService.Common.Model;

namespace In.ProjectEKA.HipService.Consent
{
    public class ConsentArtefactRequest
    {
        public string Signature { get; set; }
        public ConsentArtefact ConsentDetail { get; set; }
        public ConsentStatus Status { get; set; }
        public string ConsentId { get; set; }

        public ConsentArtefactRequest(string signature, ConsentArtefact consentDetail, ConsentStatus status,
            string consentId)
        {
            Signature = signature;
            ConsentDetail = consentDetail;
            Status = status;
            ConsentId = consentId;
        }
    }
}