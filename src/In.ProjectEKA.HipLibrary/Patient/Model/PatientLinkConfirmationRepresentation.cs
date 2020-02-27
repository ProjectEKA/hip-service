namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    using System.Text.Json.Serialization;
    using System.Xml.Serialization;

    public class PatientLinkConfirmationRepresentation
    {
        [JsonPropertyName("patient")]
        [XmlElement("patient")]
        public LinkConfirmationRepresentation Patient { get; }

        public PatientLinkConfirmationRepresentation(LinkConfirmationRepresentation patient)
        {
            Patient = patient;
        }
    }
}