namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    using System.Collections.Generic;

    public class PatientEnquiryRepresentation
    {
        public string ReferenceNumber { get; set; }

        public string Display { get; set; }

        public IEnumerable<CareContextRepresentation> CareContexts { get; }

        public IEnumerable<string> MatchedBy { get; }

        public PatientEnquiryRepresentation(
            string referenceNumber,
            string display,
            IEnumerable<CareContextRepresentation> careContexts,
            IEnumerable<string> matchedBy)
        {
            ReferenceNumber = referenceNumber;
            Display = display;
            CareContexts = careContexts;
            MatchedBy = matchedBy;
        }
    }
}