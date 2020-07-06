namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public class CareContextEnquiry
    {
        public string ReferenceNumber { get; set; }

        public CareContextEnquiry(string referenceNumber)
        {
            ReferenceNumber = referenceNumber;
        }
    }
}