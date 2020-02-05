namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    using System.Text.Json.Serialization;
    using System.Xml.Serialization;

    public class PatientLinkEnquiryRepresentation
    {
        [JsonPropertyName("link")]
        [XmlElement("link")]
        public LinkEnquiryRepresentation Link { get; }
        
        public PatientLinkEnquiryRepresentation(LinkEnquiryRepresentation link)
        {
            Link = link;
        }
    }
}