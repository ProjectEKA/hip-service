namespace In.ProjectEKA.HipService.Link.Model
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class LinkEnquires
    {
        public LinkEnquires()
        {
        }

        public LinkEnquires(
            string patientReferenceNumber,
            string linkReferenceNumber,
            string consentManagerId,
            string consentManagerUserId,
            string dateTimeStamp,
            ICollection<CareContext> careContexts)
        {
            PatientReferenceNumber = patientReferenceNumber;
            LinkReferenceNumber = linkReferenceNumber;
            ConsentManagerId = consentManagerId;
            ConsentManagerUserId = consentManagerUserId;
            DateTimeStamp = dateTimeStamp;
            CareContexts = careContexts;
        }

        public string PatientReferenceNumber { get; set; }

        [Key]
        public string LinkReferenceNumber { get; set; }

        public string ConsentManagerId { get; set; }

        public string ConsentManagerUserId { get; set; }

        public string DateTimeStamp { get; set; }

        public virtual ICollection<CareContext> CareContexts { get; set; }
    }
}