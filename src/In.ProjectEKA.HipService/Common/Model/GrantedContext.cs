namespace In.ProjectEKA.HipService.Common.Model
{
    public class GrantedContext
    {
        public string PatientReference { get; }
        
        public string CareContextReference { get; }

        public GrantedContext(string patientReference, string careContextReference)
        {
            PatientReference = patientReference;
            CareContextReference = careContextReference;
        }
    }
}