namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public class GrantedContext
    {
        public GrantedContext(string patientReference, string careContextReference)
        {
            PatientReference = patientReference;
            CareContextReference = careContextReference;
        }

        public string PatientReference { get; }

        public string CareContextReference { get; }
    }
}