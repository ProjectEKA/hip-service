namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    using System.Collections.Generic;

    public class Patient
    {
        public string Identifier { get; set; }

        public string Uuid { get; set; }

        public Gender? Gender { get; set; }

        public string PhoneNumber { get; set; }

        public string Name { get; set; }

        public ushort? YearOfBirth { get; set; }

        public IEnumerable<CareContextRepresentation> CareContexts { get; set; }

        public PatientEnquiryRepresentation ToPatientEnquiryRepresentation(
            IEnumerable<CareContextRepresentation> unlinkedCareContexts)
        {
            return new PatientEnquiryRepresentation(
                Identifier,
                Name,
                unlinkedCareContexts,
                new[] {Match.ConsentManagerUserId.ToString()});
        }
    }
}