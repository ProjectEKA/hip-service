namespace In.ProjectEKA.HipService.Discovery
{
    using HipLibrary.Patient.Model;

    public class DiscoveryRepresentation
    {
        public DiscoveryRepresentation(PatientEnquiryRepresentation patient)
        {
            Patient = patient;
        }

        public PatientEnquiryRepresentation Patient { get; }
    }
}