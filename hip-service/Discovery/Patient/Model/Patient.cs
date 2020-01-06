using System;
using System.Collections.Generic;
using hip_service.Discovery.Patient.models;

namespace hip_service.Discovery.Patient.Model
{
    public class Patient
    {
        public string Identifier { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public IEnumerable<CareContext> CareContexts { get; set; }
    }
}