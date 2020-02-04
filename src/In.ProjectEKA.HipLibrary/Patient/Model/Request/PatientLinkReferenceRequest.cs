namespace In.ProjectEKA.HipLibrary.Patient.Model.Request
{
    using System.Text.Json.Serialization;
    using System.Xml.Serialization;

    public class PatientLinkReferenceRequest
    {
        [JsonPropertyName("transactionId")]
        [XmlElement("transactionId")]
        public string TransactionId { get; }
        
        [JsonPropertyName("patient")]
        [XmlElement("patient")]
        public Link Patient { get; }
        
        public PatientLinkReferenceRequest(string transactionId, Link patient)
        {
            TransactionId = transactionId;
            Patient = patient;
        }
    }
}