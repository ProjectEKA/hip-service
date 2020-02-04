namespace In.ProjectEKA.HipLibrary.Patient
{
    using System;
    using System.Threading.Tasks;
    using Model.Request;
    using Model.Response;

    public interface ILink
    {
        Task<Tuple<PatientLinkReferenceResponse, ErrorResponse>> LinkPatients(PatientLinkReferenceRequest request);
        Task<Tuple<PatientLinkResponse, ErrorResponse>> VerifyAndLinkCareContext(PatientLinkRequest request);
    }
}