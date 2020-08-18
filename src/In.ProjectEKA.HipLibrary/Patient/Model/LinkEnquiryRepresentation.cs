namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public class LinkEnquiryRepresentation
    {
        public LinkEnquiryRepresentation()
        {
        }

        public LinkEnquiryRepresentation(string referenceNumber, string authenticationType, LinkReferenceMeta meta)
        {
            ReferenceNumber = referenceNumber;
            AuthenticationType = authenticationType;
            Meta = meta;
        }

        public string ReferenceNumber { get; }

        public string AuthenticationType { get; }

        public LinkReferenceMeta Meta { get; }
    }
}