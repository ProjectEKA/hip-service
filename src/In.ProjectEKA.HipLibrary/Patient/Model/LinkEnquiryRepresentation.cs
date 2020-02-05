namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    using System.Text.Json.Serialization;
    using System.Xml.Serialization;

    public class LinkEnquiryRepresentation
    { 
        [JsonPropertyName("referenceNumber")]
        [XmlElement("referenceNumber")]
        public string ReferenceNumber { get; }

        [JsonPropertyName("authenticationType")]
        [XmlElement("authenticationType")]
        public string AuthenticationType { get; }

        [JsonPropertyName("meta")]
        [XmlElement("meta")]
        public LinkReferenceMeta Meta { get; }
        
        public LinkEnquiryRepresentation(string referenceNumber, string authenticationType, LinkReferenceMeta meta)
        {
            ReferenceNumber = referenceNumber;
            AuthenticationType = authenticationType;
            Meta = meta;
        }
    }
}