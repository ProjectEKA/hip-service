namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public class PatientLinkEnquiryRepresentation
    {
        public PatientLinkEnquiryRepresentation()
        {
        }

        public PatientLinkEnquiryRepresentation(LinkEnquiryRepresentation link)
        {
            Link = link;
        }

        public LinkEnquiryRepresentation Link { get; }
    }
}