using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace hip_service.Link.Patient
{
    public class LinkToken
    {
        [JsonPropertyName("token")]
        [XmlElement("token")]
        public string Token { get; }

        public LinkToken(string token)
        {
            Token = token;
        }
    }
}