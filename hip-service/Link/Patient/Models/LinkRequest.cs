using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace hip_service.Link.Patient.Models
{
    public class LinkRequest
    {
        public string PatientReferenceNumber { get; set; }
        [Key]
        public string LinkReferenceNumber { get; set; }
        public string ConsentManagerId { get; set; }
        public string ConsentManagerUserId { get; set; }
        public string DateTimeStamp { get; set; }
        public ICollection<LinkedCareContext> CareContexts { get; set; }

        public LinkRequest()
        {
        }

        public LinkRequest(string patientReferenceNumber, string linkReferenceNumber, string consentManagerId, string consentManagerUserId, string dateTimeStamp, ICollection<LinkedCareContext> careContexts)
        {
            PatientReferenceNumber = patientReferenceNumber;
            LinkReferenceNumber = linkReferenceNumber;
            ConsentManagerId = consentManagerId;
            ConsentManagerUserId = consentManagerUserId;
            DateTimeStamp = dateTimeStamp;
            CareContexts = careContexts;
        }
    }
}