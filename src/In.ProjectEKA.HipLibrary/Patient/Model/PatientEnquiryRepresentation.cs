namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    using System.Collections.Generic;

    public class PatientEnquiryRepresentation
    {
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

        public string ReferenceNumber { get; }

        public string Display { get; }

        public IEnumerable<CareContextRepresentation> CareContexts { get; }

        public IEnumerable<string> MatchedBy { get; }
    }
}