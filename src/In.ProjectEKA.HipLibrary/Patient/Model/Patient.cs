namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    public class Patient
    {
        public string Identifier { get; set; }

        public string Gender { get; set; }

        public string PhoneNumber { get; set; }

        public string Name { get; set; }

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