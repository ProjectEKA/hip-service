namespace In.ProjectEKA.HipService.Link
{
    using System;
    using System.Threading.Tasks;
    using HipLibrary.Patient.Model;

    public interface ILink
    {
        Task<Tuple<PatientLinkEnquiryRepresentation, ErrorRepresentation>> LinkPatients(PatientLinkEnquiry request);

        Task<Tuple<PatientLinkConfirmationRepresentation, ErrorRepresentation>>
            VerifyAndLinkCareContext(LinkConfirmationRequest request);
    }
}