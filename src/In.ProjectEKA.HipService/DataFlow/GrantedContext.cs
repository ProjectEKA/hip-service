namespace In.ProjectEKA.HipService.DataFlow
{
    public class GrantedContext
    {
        public string PatientReferenceNumber { get; }
        public string CareContextReferenceNumber { get; }

        public GrantedContext(string patientReferenceNumber, string careContextReferenceNumber)
        {
            PatientReferenceNumber = patientReferenceNumber;
            CareContextReferenceNumber = careContextReferenceNumber;
        }
    }
}