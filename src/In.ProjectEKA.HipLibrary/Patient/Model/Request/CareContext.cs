namespace In.ProjectEKA.HipLibrary.Patient.Model.Request
{
    using System.Text.Json.Serialization;
    using System.Xml.Serialization;

    public class CareContext
    {
        [JsonPropertyName("referenceNumber")]
        [XmlElement("referenceNumber")]
        public string ReferenceNumber { get; }

        public CareContext(string referenceNumber)
        {
            ReferenceNumber = referenceNumber;
        }
    }
}