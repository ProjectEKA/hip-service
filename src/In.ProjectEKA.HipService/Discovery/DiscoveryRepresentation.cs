using In.ProjectEKA.HipLibrary.Patient.Model;

namespace In.ProjectEKA.HipService.Discovery
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