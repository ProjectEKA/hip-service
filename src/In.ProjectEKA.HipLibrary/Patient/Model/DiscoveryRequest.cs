namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public class DiscoveryRequest
    {
        public PatientEnquiry Patient { get; }

        public string RequestId { get; }

        public DiscoveryRequest(PatientEnquiry patient, string requestId)
        {
            Patient = patient;
            RequestId = requestId;
        }
    }
}