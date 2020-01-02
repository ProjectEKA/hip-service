using System;
using System.Collections.Generic;

namespace hip_service.Discovery.Patient.models
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