namespace In.ProjectEKA.HipLibrary.Patient.Model.Response
{
    using System.Text.Json.Serialization;
    using System.Xml.Serialization;

    public class PatientLinkReferenceResponse
    {
        [JsonPropertyName("link")]
        [XmlElement("link")]
        public LinkReference Link { get; }
        
        public PatientLinkReferenceResponse(LinkReference link)
        {
            Link = link;
        }
    }
}