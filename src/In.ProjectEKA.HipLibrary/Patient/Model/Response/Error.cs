namespace In.ProjectEKA.HipLibrary.Patient.Model.Response
{
    using System.Text.Json.Serialization;
    using System.Xml.Serialization;

    public class Error
    {
        [JsonPropertyName("code")]
        [XmlElement("code")]
        public ErrorCode Code { get; }
        
        [JsonPropertyName("message")]
        [XmlElement("message")]
        public string Message { get; } 
        
        public Error(ErrorCode code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}