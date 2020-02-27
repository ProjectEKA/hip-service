namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    using System.Text.Json.Serialization;
    using System.Xml.Serialization;

    public class Identifier
    {
        [JsonPropertyName("type")]
        [XmlElement("type")]
        public IdentifierType Type { get; }

        [JsonPropertyName("value")]
        [XmlElement("value")]
        public string Value { get; }

        public Identifier(IdentifierType type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}