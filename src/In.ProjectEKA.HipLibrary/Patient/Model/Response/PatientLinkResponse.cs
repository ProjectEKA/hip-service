namespace In.ProjectEKA.HipLibrary.Patient.Model.Response
{
    using System.Text.Json.Serialization;
    using System.Xml.Serialization;

    public class PatientLinkResponse
    {
        [JsonPropertyName("patient")]
        [XmlElement("patient")]
        public LinkPatient Patient { get; }

        public PatientLinkResponse(LinkPatient patient)
        {
            Patient = patient;
        }
    }
}