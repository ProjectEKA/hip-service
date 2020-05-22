using In.ProjectEKA.HipLibrary.Patient.Model;

namespace In.ProjectEKA.HipService.Gateway.Model
{
    public class GatewayLinkResponse
    {
        private readonly PatientLinkEnquiryRepresentation patientLinkEnquiryRepresentation;
        private readonly ErrorRepresentation errorRepresentation;

        public GatewayLinkResponse(PatientLinkEnquiryRepresentation patientLinkEnquiryRepresentation,
            ErrorRepresentation errorRepresentation)
        {
            this.patientLinkEnquiryRepresentation = patientLinkEnquiryRepresentation;
            this.errorRepresentation = errorRepresentation;
        }
    }
}