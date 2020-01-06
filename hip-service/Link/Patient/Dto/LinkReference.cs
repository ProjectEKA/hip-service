using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using HipLibrary.Patient.Models.Request;

namespace hip_service.Link.Patient.Dto
{
    public class LinkReference
    {
        [JsonPropertyName("consentManagerUserId")]
        [XmlElement("consentManagerUserId")]
        public string ConsentManagerUserId { get; }

        [JsonPropertyName("referenceNumber")]
        [XmlElement("referenceNumber")]
        public string ReferenceNumber { get; }

        [JsonPropertyName("careContexts")]
        [XmlElement("careContexts")]
        public IEnumerable<CareContext> CareContexts { get; }

        public LinkReference(string consentManagerUserId, string referenceNumber, IEnumerable<CareContext> careContexts)
        {
            ConsentManagerUserId = consentManagerUserId;
            ReferenceNumber = referenceNumber;
            CareContexts = careContexts;
        }
    }
}