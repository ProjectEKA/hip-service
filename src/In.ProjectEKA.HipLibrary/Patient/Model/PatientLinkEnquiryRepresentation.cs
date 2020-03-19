namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public class PatientLinkEnquiryRepresentation
    {
        public LinkEnquiryRepresentation Link { get; }

        public PatientLinkEnquiryRepresentation(LinkEnquiryRepresentation link)
        {
            Link = link;
        }
    }
}