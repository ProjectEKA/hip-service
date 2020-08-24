namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public class CareContextEnquiry
    {
        public CareContextEnquiry(string referenceNumber)
        {
            ReferenceNumber = referenceNumber;
        }

        public string ReferenceNumber { get; }
    }
}