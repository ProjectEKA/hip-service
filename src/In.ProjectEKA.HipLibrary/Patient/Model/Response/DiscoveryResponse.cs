namespace In.ProjectEKA.HipLibrary.Patient.Model.Response
{
    using System.Text.Json.Serialization;
    using System.Xml.Serialization;

    public class DiscoveryResponse
    {
        [JsonPropertyName("patient")]
        [XmlElement("patient")]
        public Patient Patient { get; }
        public DiscoveryResponse(Patient patient)
        {
            Patient = patient;
        }
    }
}