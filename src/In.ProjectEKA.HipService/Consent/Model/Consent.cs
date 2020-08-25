namespace In.ProjectEKA.HipService.Consent.Model
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Common.Model;

    public class Consent
    {
        public Consent(string consentArtefactId,
            ConsentArtefact consentArtefact,
            string signature,
            ConsentStatus status,
            string consentManagerId)
        {
            ConsentArtefactId = consentArtefactId;
            ConsentArtefact = consentArtefact;
            Signature = signature;
            Status = status;
            ConsentManagerId = consentManagerId;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public string ConsentArtefactId { get; set; }

        public ConsentArtefact ConsentArtefact { get; set; }

        public string Signature { get; set; }

        public ConsentStatus Status { get; set; }

        public string ConsentManagerId { get; set; }
    }
}