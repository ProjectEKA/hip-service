namespace In.ProjectEKA.HipLibrary.Patient
{
    using System;
    using System.Threading.Tasks;
    using Model;

    public interface ILink
    {
        Task<Tuple<PatientLinkEnquiryRepresentation, ErrorRepresentation>> LinkPatients(PatientLinkEnquiry request);
        Task<Tuple<PatientLinkConfirmationRepresentation, ErrorRepresentation>> VerifyAndLinkCareContext(LinkConfirmationRequest request);
    }
}