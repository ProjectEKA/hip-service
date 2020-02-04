namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    using System.Text.Json.Serialization;
    using System.Xml.Serialization;

    public class ErrorRepresentation
    {
        [JsonPropertyName("error")]
        [XmlElement("error")]
        public Error Error { get; }
        
        public ErrorRepresentation(Error error)
        {
            Error = error;
        }
    }
}