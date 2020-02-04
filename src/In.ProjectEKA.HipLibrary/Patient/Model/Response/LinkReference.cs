namespace In.ProjectEKA.HipLibrary.Patient.Model.Response
{
    using System;
    using System.Text.Json.Serialization;
    using System.Xml.Serialization;

    public class LinkReference
    { 
        [JsonPropertyName("referenceNumber")]
        [XmlElement("referenceNumber")]
        public String ReferenceNumber { get; }

        [JsonPropertyName("authenticationType")]
        [XmlElement("authenticationType")]
        public string AuthenticationType { get; }

        [JsonPropertyName("meta")]
        [XmlElement("meta")]
        public LinkReferenceMeta Meta { get; }
        
        public LinkReference(string referenceNumber, string authenticationType, LinkReferenceMeta meta)
        {
            ReferenceNumber = referenceNumber;
            AuthenticationType = authenticationType;
            Meta = meta;
        }
    }
}