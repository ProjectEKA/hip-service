namespace In.ProjectEKA.HipLibrary.Patient.Model.Request
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
    using System.Xml.Serialization;

    public class Link
    {
        [JsonPropertyName("consentManagerId")]
        [XmlElement("consentManagerId")]
        public string ConsentManagerId { get; }

        [JsonPropertyName("consentManagerUserId")]
        [XmlElement("consentManagerUserId")]
        public string ConsentManagerUserId { get; }

        [JsonPropertyName("referenceNumber")]
        [XmlElement("referenceNumber")]
        public string ReferenceNumber { get; }

        [JsonPropertyName("careContexts")]
        [XmlElement("careContexts")]
        public IEnumerable<CareContext> CareContexts { get; }

        public Link(string consentManagerId, string consentManagerUserId, string referenceNumber, IEnumerable<CareContext> careContexts)
        {
            ConsentManagerId = consentManagerId;
            ConsentManagerUserId = consentManagerUserId;
            ReferenceNumber = referenceNumber;
            CareContexts = careContexts;
        }
    }
}