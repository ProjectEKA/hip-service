namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    using System.Text.Json.Serialization;
    using System.Xml.Serialization;

    public class DiscoveryRepresentation
    {
        [JsonPropertyName("patient")]
        [XmlElement("patient")]
        public PatientEnquiryRepresentation Patient { get; }
        
        public DiscoveryRepresentation(PatientEnquiryRepresentation patient)
        {
            Patient = patient;
        }
    }
}