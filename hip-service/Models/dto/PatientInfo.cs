using System;
using hip_library.Patient.models.domain;

namespace hip_service.Models.dto
{
    public class PatientInfo
    {
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CaseId { get; set; }
        public string Gender { get; set; }
        public DateTime BirthDate { get; set; }
        
        public AddressInfo Address { get; set; }
        public ContactInfo Contact { get; set; }
    }
}