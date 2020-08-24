namespace In.ProjectEKA.FHIRHip.Discovery.Model
{
    using System.Collections.Generic;
    using HipLibrary.Patient.Model;

    public class PatientResponse
    {
        public string PhoneNumber { get; set; }
        public string Identifier { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string CaseReferenceNumber { get; set; }
        public Gender Gender { get; set; }
        public ushort YearOfBirth { get; set; }
        public IEnumerable<CareContextRepresentation> CareContexts { get; set; }
    }
}