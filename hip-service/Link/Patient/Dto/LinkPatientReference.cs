using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace hip_service.Link.Patient.Dto
{
    public class LinkPatientReference
    {
        [JsonPropertyName("transactionId")]
        [XmlElement("transactionId")]
        public string TransactionId { get; }

        [JsonPropertyName("patient")]
        [XmlElement("patient")]
        public LinkReference LinkReference { get; }

        public LinkPatientReference(string transactionId, LinkReference linkReference)
        {
            TransactionId = transactionId;
            LinkReference = linkReference;
        }
    }
}