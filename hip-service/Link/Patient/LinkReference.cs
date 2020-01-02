using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using hip_library.Patient.models;

namespace hip_service.Link.Patient
{
    public class LinkReference
    {
        [JsonPropertyName("consentManagerUserId")]
        [XmlElement("consentManagerUserId")]
        public string ConsentManagerUserId { get; }

        [JsonPropertyName("patientReferenceNumber")]
        [XmlElement("patientReferenceNumber")]
        public string PatientReferenceNumber { get; }

        [JsonPropertyName("careContexts")]
        [XmlElement("careContexts")]
        public IEnumerable<CareContext> CareContexts { get; }

        public LinkReference(string consentManagerUserId, string patientReferenceNumber, IEnumerable<CareContext> careContexts)
        {
            ConsentManagerUserId = consentManagerUserId;
            PatientReferenceNumber = patientReferenceNumber;
            CareContexts = careContexts;
        }
    }
}