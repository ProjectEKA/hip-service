namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public class CareContextRepresentation
    {
        public CareContextRepresentation(string referenceNumber, string display)
        {
            ReferenceNumber = referenceNumber;
            Display = display;
        }

        public string ReferenceNumber { get; }

        public string Display { get; }
    }
}