using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hip_service.Link.Patient.Models
{
    public class LinkedCareContext
    {
        public string CareContextNumber { get; set; }
        
        [ForeignKey("LinkReferenceNumber")]
        public string LinkReferenceNumber { get; set; }
        public LinkRequest LinkRequest { get; set; }

        public LinkedCareContext()
        {
        }

        public LinkedCareContext(string careContextNumber)
        {
            CareContextNumber = careContextNumber;
        }
    }
}