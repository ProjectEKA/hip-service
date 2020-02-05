namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    using System.Text.Json.Serialization;
    using System.Xml.Serialization;

    public class DiscoveryRequest
    {
        [JsonPropertyName("patient")]
        [XmlElement("patient")]
        public PatientEnquiry Patient { get; }

        [JsonPropertyName("transactionId")]
        [XmlElement("transactionId")]
        public string TransactionId { get; }

        public DiscoveryRequest(PatientEnquiry patient, string transactionId)
        {
            Patient = patient;
            TransactionId = transactionId;
        }
    }
}