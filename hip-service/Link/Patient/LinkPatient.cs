using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using health_information_provider_library.Patient.models;
using hip_library.Patient;
using hip_library.Patient.models;
using hip_library.Patient.models.dto;
using hip_service.Discovery.Patient.models;

namespace hip_service.Link.Patient
{
    public class LinkPatient: ILink
    {
        private readonly Patient.ILinkPatientRepository linkPatientRepository;
        private readonly PatientRepository _patientRepository;

        public LinkPatient(ILinkPatientRepository linkPatientRepository, PatientRepository patientRepository)
        {
            this.linkPatientRepository = linkPatientRepository;
            _patientRepository = patientRepository;
        }

        public async Task<Tuple<PatientLinkReferenceResponse, Error>> LinkPatients(PatientLinkReferenceRequest request)
        {
            if (_patientRepository.GetPatientInfoWithReferenceNumber(request.PatientReferenceNumber) == null)
            {
                return new Tuple<PatientLinkReferenceResponse, Error>
                    (null, new Error(ErrorCode.NoPatientFound, "No patient found"));
            }

            var programs = new List<ProgramInfo>();
            foreach (var careContext in request.CareContexts)
            {
                programs.Add(_patientRepository.GetProgramInfo(request.PatientReferenceNumber,
                    careContext.ReferenceNumber));
            }

            if (programs.Count != request.CareContexts.Count())
            {
                return new Tuple<PatientLinkReferenceResponse, Error>
                    (null, new Error(ErrorCode.CareContextNotFound, "Care context not found for given patient"));
            }           
            
            var linkRefNumber = Guid.NewGuid().ToString();
            
            // method call for generating the OTP

            var (_, exception) = await linkPatientRepository.SaveLinkPatientDetails(linkRefNumber, request.ConsentManagerId,
                request.ConsentManagerUserId, request.PatientReferenceNumber, request.CareContexts
                    .Select(context => context.ReferenceNumber).ToArray());

            if (exception != null)
            {
                return new Tuple<PatientLinkReferenceResponse, Error>
                    (null, new Error(ErrorCode.CareContextNotFound, "Unable to store data to Database"));
            }
            
            var expiry = new DateTime (DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 
                    DateTime.Now.Hour, DateTime.Now.Minute+1, DateTime.Now.Second).ToUniversalTime().
                ToString("yyyy-MM-ddTHH:mm:ssZ"); 

            var meta = new LinkReferenceMeta(nameof(LinkReferenceMode.Mobile).ToUpper(), 
                _patientRepository.GetPatientInfoWithReferenceNumber(request.PatientReferenceNumber).PhoneNumber, expiry);
            var patientLinkReferenceResponse = new PatientLinkReferenceResponse(linkRefNumber, 
                nameof(AuthenticationType.Mediated).ToUpper(), meta);

            return new Tuple<PatientLinkReferenceResponse, Error>(patientLinkReferenceResponse, null);
        }

        public Task<Tuple<PatientLinkResponse, Error>> VerifyAndLinkCareContext(PatientLinkRequest request)
        {
            throw new NotImplementedException();
        }

    }
}