namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    using System.Text.Json.Serialization;
    using System.Xml.Serialization;

    public class CareContextEnquiry
    {
        [JsonPropertyName("referenceNumber")]
        [XmlElement("referenceNumber")]
        public string ReferenceNumber { get; }

        public CareContextEnquiry(string referenceNumber)
        {
            ReferenceNumber = referenceNumber;
        }
    }
}