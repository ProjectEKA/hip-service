namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    using System.Text.Json.Serialization;
    using System.Xml.Serialization;

    public class LinkReferenceMeta
    {
        [JsonPropertyName("communicationMedium")]
        [XmlElement("communicationMedium")]
        public string CommunicationMedium { get; }

        [JsonPropertyName("communicationHint")]
        [XmlElement("communicationHint")]
        public string CommunicationHint { get; }

        [JsonPropertyName("communicationExpiry")]
        [XmlElement("communicationExpiry")]
        public string CommunicationExpiry { get; }


        public LinkReferenceMeta(string communicationMedium, string communicationHint, string communicationExpiry)
        {
            CommunicationMedium = communicationMedium;
            CommunicationHint = communicationHint;
            CommunicationExpiry = communicationExpiry;
        }
    }
}