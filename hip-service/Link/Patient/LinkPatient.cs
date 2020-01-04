using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using health_information_provider_library.Patient.models;
using hip_library.Patient;
using hip_library.Patient.models;
using hip_library.Patient.models.dto;
using hip_service.Discovery.Patient.models;
using hip_service.OTP;

namespace hip_service.Link.Patient
{
    public class LinkPatient: ILink
    {
        private readonly ILinkPatientRepository linkPatientRepository;
        private readonly PatientRepository _patientRepository;
        private readonly IPatientVerification _patientVerification;

        public LinkPatient(ILinkPatientRepository linkPatientRepository, PatientRepository patientRepository,
            IPatientVerification patientVerification)
        {
            this.linkPatientRepository = linkPatientRepository;
            _patientRepository = patientRepository;
            _patientVerification = patientVerification;
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
            var session = new Session(linkRefNumber, new Communication(LinkReferenceMode.Mobile, ""));

            if (await _patientVerification.GenerateVerificationToken(session) != null)
            {
                return new Tuple<PatientLinkReferenceResponse, Error>
                    (null, new Error(ErrorCode.OtpInValid, "Unable to create token"));
            }
            
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

        public async Task<Tuple<PatientLinkResponse, Error>> VerifyAndLinkCareContext(PatientLinkRequest request)
        {
            if (await _patientVerification.AuthenticateVerificationToken(request.LinkReferenceNumber, request.Token) != null)
            {
                return new Tuple<PatientLinkResponse, Error>
                    (null, new Error(ErrorCode.OtpInValid, "Otp Invalid"));   
            }

            var (linkRequest, exception) =
                await linkPatientRepository.GetPatientReferenceNumber(request.LinkReferenceNumber);
            if (exception != null)
            {
                return new Tuple<PatientLinkResponse, Error>
                    (null, new Error(ErrorCode.NoPatientFound, "No request found"));
            }

            var patientInfo = _patientRepository.GetPatientInfoWithReferenceNumber(linkRequest.PatientReferenceNumber);
            
            var representations = linkRequest.CareContexts
                .Where(careContext =>
                    patientInfo.Programs.Any(info => info.ReferenceNumber == careContext.CareContextNumber))
                .Select( context => new CareContextRepresentation(context.CareContextNumber, 
                    patientInfo.Programs.First(info => info.ReferenceNumber == context.CareContextNumber).Description));
            
            var patientLinkResponse = new PatientLinkResponse(linkRequest.PatientReferenceNumber,
                representations);
             
            return new Tuple<PatientLinkResponse, Error>
                (patientLinkResponse, null);
            
        }

    }
}