namespace In.ProjectEKA.HipLibrary.Patient.Model.Response
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
    using System.Xml.Serialization;

    public class Patient
    {
        [JsonPropertyName("referenceNumber")]
        [XmlElement("referenceNumber")]
        public string ReferenceNumber { get; }

        [JsonPropertyName("display")]
        [XmlElement("display")]
        public string Display { get; }

        [JsonPropertyName("careContexts")]
        [XmlElement("careContexts")]
        public IEnumerable<CareContextRepresentation> CareContexts { get; }

        [JsonPropertyName("matchedBy")]
        [XmlElement("matchedBy")]
        public IEnumerable<string> MatchedBy { get; }

        public Patient(string referenceNumber, string display, IEnumerable<CareContextRepresentation> careContexts,
            IEnumerable<string> matchedBy)
        {
            ReferenceNumber = referenceNumber;
            Display = display;
            CareContexts = careContexts;
            MatchedBy = matchedBy;
        }
    }
}