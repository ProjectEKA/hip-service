using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using hip_service.OTP;
using HipLibrary.Patient;
using HipLibrary.Patient.Models;
using HipLibrary.Patient.Models.Request;
using HipLibrary.Patient.Models.Response;

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

        public async Task<Tuple<PatientLinkReferenceResponse, ErrorResponse>> LinkPatients(PatientLinkReferenceRequest request)
        {

            var requestPatient = request.Patient;
            var patient = _patientRepository.GetPatientInfoWithReferenceNumber(requestPatient.ReferenceNumber);
            
            if (patient == null)
            {
                return new Tuple<PatientLinkReferenceResponse, ErrorResponse>
                    (null, new ErrorResponse(new Error(ErrorCode.NoPatientFound, "No patient found")));
            }
            var programs = new List<hip_service.Discovery.Patient.models.CareContext>();
            foreach (var careContext in requestPatient.CareContexts)
            {
                if (_patientRepository.GetProgramInfo(requestPatient.ReferenceNumber,
                        careContext.ReferenceNumber) != null)
                {
                    programs.Add(_patientRepository.GetProgramInfo(requestPatient.ReferenceNumber,
                        careContext.ReferenceNumber));
                }
            }

            if (programs.Count != requestPatient.CareContexts.Count())
            {
                return new Tuple<PatientLinkReferenceResponse, ErrorResponse>
                    (null, new ErrorResponse(new Error(ErrorCode.CareContextNotFound, "Care context not found for given patient")));
            }           
            
            var linkRefNumber = Guid.NewGuid().ToString();
            
            // method call for generating the OTP
            var session = new Session(linkRefNumber, new Communication(IdentifierType.MOBILE,patient.PhoneNumber));

            if (await _patientVerification.GenerateVerificationToken(session) != null)
            {
                return new Tuple<PatientLinkReferenceResponse, ErrorResponse>
                    (null, new ErrorResponse(new Error(ErrorCode.OtpInValid, "Unable to create token")));
            }
            
            var (_, exception) = await linkPatientRepository.SaveLinkPatientDetails(linkRefNumber, requestPatient.ConsentManagerId,
                requestPatient.ConsentManagerUserId, requestPatient.ReferenceNumber, requestPatient.CareContexts
                    .Select(context => context.ReferenceNumber).ToArray());

            if (exception != null)
            {
                return new Tuple<PatientLinkReferenceResponse, ErrorResponse>
                    (null, new ErrorResponse(new Error(ErrorCode.CareContextNotFound, "Unable to store data to Database")));
            }

            const string dateTimeFormat = "yyyy-MM-ddTHH:mm:ssZ";
            var expiry = new DateTime (DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 
                    DateTime.Now.Hour, DateTime.Now.Minute+1, DateTime.Now.Second).ToUniversalTime().
                ToString(dateTimeFormat); 

            var meta = new LinkReferenceMeta(nameof(IdentifierType.MOBILE).ToUpper(), 
                _patientRepository.GetPatientInfoWithReferenceNumber(requestPatient.ReferenceNumber).PhoneNumber, expiry);
            var patientLinkReferenceResponse = new PatientLinkReferenceResponse(new LinkReference(linkRefNumber,"MEDIATED",meta));

            return new Tuple<PatientLinkReferenceResponse, ErrorResponse>(patientLinkReferenceResponse, null);
        }

        public async Task<Tuple<PatientLinkResponse, ErrorResponse>> VerifyAndLinkCareContext(PatientLinkRequest request)
        {
            if (await _patientVerification.AuthenticateVerificationToken(request.LinkReferenceNumber, request.Token) != null)
            {
                return new Tuple<PatientLinkResponse, ErrorResponse>
                    (null, new ErrorResponse(new  Error(ErrorCode.OtpInValid, "Otp Invalid")));   
            }

            var (linkRequest, exception) =
                await linkPatientRepository.GetPatientReferenceNumber(request.LinkReferenceNumber);
       
            if (exception != null)
            {
                return new Tuple<PatientLinkResponse, ErrorResponse>
                    (null, new ErrorResponse(new Error(ErrorCode.NoPatientFound, "No request found")));
            }

            var patientInfo = _patientRepository.GetPatientInfoWithReferenceNumber(linkRequest.PatientReferenceNumber);

            var representations = linkRequest.CareContexts
                .Where(careContext =>
                    patientInfo.CareContexts.Any(info => info.ReferenceNumber == careContext.CareContextNumber))
                .Select( context => new CareContextRepresentation(context.CareContextNumber, 
                    patientInfo.CareContexts.First(info => info.ReferenceNumber == context.CareContextNumber).Description));
            
            var patientLinkResponse = new PatientLinkResponse(new HipLibrary.Patient.Models.Response.LinkPatient(linkRequest.PatientReferenceNumber
                    ,patientInfo.FirstName + " " + patientInfo.LastName,representations));

            return new Tuple<PatientLinkResponse, ErrorResponse>
                (patientLinkResponse, null);
            
        }

    }
}