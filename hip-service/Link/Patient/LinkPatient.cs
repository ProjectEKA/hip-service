using System;
using System.Linq;
using System.Threading.Tasks;
using health_information_provider_library.Patient.models;
using hip_library.Patient;
using hip_library.Patient.models;
using hip_library.Patient.models.dto;

namespace hip_service.Link.Patient
{
    public class LinkPatient: ILink
    {
        private readonly Patient.ILinkPatientRepository linkPatientRepository;

        public LinkPatient(ILinkPatientRepository linkPatientRepository)
        {
            this.linkPatientRepository = linkPatientRepository;
        }

        public Task<Tuple<PatientLinkReferenceResponse, Error>> LinkPatients(PatientLinkReferenceRequest request)
        {
            return linkPatientRepository.LinkPatient(request.ConsentManagerId, request.ConsentManagerUserId,
                request.PatientReferenceNumber, request.CareContexts
                    .Select(context => context.ReferenceNumber).ToArray());
        }

        public Task<Tuple<PatientLinkResponse, Error>> VerifyAndLinkCareContext(PatientLinkRequest request)
        {
            throw new NotImplementedException();
        }

    }
}