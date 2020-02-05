namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    using System.Text.Json.Serialization;
    using System.Xml.Serialization;

    public class PatientLinkEnquiry
    {
        [JsonPropertyName("transactionId")]
        [XmlElement("transactionId")]
        public string TransactionId { get; }
        
        [JsonPropertyName("patient")]
        [XmlElement("patient")]
        public LinkEnquiry Patient { get; }
        
        public PatientLinkEnquiry(string transactionId, LinkEnquiry patient)
        {
            TransactionId = transactionId;
            Patient = patient;
        }
    }
}