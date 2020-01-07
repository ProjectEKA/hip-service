using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace hip_service.Link.Patient
{
    public class PatientLinkRequest
    {
        public string Token { get; }

        public PatientLinkRequest(string token)
        {
            Token = token;
        }
    }
}