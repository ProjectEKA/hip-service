namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    using System.Text.Json.Serialization;
    using System.Xml.Serialization;

    public class LinkEnquiryRepresentation
    {
        public string ReferenceNumber { get; }

        public string AuthenticationType { get; }

        public LinkReferenceMeta Meta { get; }

        public LinkEnquiryRepresentation(string referenceNumber, string authenticationType, LinkReferenceMeta meta)
        {
            ReferenceNumber = referenceNumber;
            AuthenticationType = authenticationType;
            Meta = meta;
        }
    }
}