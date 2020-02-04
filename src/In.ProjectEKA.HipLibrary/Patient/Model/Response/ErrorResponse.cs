namespace In.ProjectEKA.HipLibrary.Patient.Model.Response
{
    using System.Text.Json.Serialization;
    using System.Xml.Serialization;

    public class ErrorResponse
    {
        [JsonPropertyName("error")]
        [XmlElement("error")]
        public Error Error { get; }
        
        public ErrorResponse(Error error)
        {
            Error = error;
        }
    }
}