namespace In.ProjectEKA.HipService.Link.Model
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class LinkedAccounts
    {
        public LinkedAccounts()
        {
        }

        public LinkedAccounts(
            string patientReferenceNumber,
            string linkReferenceNumber,
            string consentManagerUserId,
            string dateTimeStamp,
            List<string> careContexts)
        {
            PatientReferenceNumber = patientReferenceNumber;
            LinkReferenceNumber = linkReferenceNumber;
            ConsentManagerUserId = consentManagerUserId;
            DateTimeStamp = dateTimeStamp;
            CareContexts = careContexts;
        }

        public string ConsentManagerUserId { get; set; }

        [Key]
        public string LinkReferenceNumber { get; set; }

        public string PatientReferenceNumber { get; set; }

        public string DateTimeStamp { get; set; }

        public List<string> CareContexts { get; set; }
    }
}