namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public class DiscoveryRepresentation
    {
        public PatientEnquiryRepresentation Patient { get; }

        public DiscoveryRepresentation(PatientEnquiryRepresentation patient)
        {
            Patient = patient;
        }
    }
}