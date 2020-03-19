namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public class CareContextEnquiry
    {
        public string ReferenceNumber { get; }

        public CareContextEnquiry(string referenceNumber)
        {
            ReferenceNumber = referenceNumber;
        }
    }
}