namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public class CareContextRepresentation
    {
        public string ReferenceNumber { get; set; }

        public string Display { get; set; }

        public CareContextRepresentation(string referenceNumber, string display)
        {
            ReferenceNumber = referenceNumber;
            Display = display;
        }
    }
}