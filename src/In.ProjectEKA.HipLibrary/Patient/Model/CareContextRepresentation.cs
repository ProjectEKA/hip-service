namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    using System.Text.Json.Serialization;
    using System.Xml.Serialization;

    public class CareContextRepresentation
    {
        public string ReferenceNumber { get; }

        public string Display { get; }

        public CareContextRepresentation(string referenceNumber, string display)
        {
            ReferenceNumber = referenceNumber;
            Display = display;
        }
    }
}