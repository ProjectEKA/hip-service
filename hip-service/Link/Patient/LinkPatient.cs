using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using hip_service.OTP;
using HipLibrary.Patient;
using HipLibrary.Patient.Models;
using HipLibrary.Patient.Models.Request;
using HipLibrary.Patient.Models.Response;
using Optional;
using CareContext = hip_service.Discovery.Patient.models.CareContext;

namespace hip_service.Link.Patient
{
    public class LinkPatient: ILink
    {
        private readonly ILinkPatientRepository linkPatientRepository;
        private readonly PatientRepository patientRepository;
        private readonly IPatientVerification patientVerification;

        public LinkPatient(ILinkPatientRepository linkPatientRepository, PatientRepository patientRepository,
            IPatientVerification patientVerification)
        {
            this.linkPatientRepository = linkPatientRepository;
            this.patientRepository = patientRepository;
            this.patientVerification = patientVerification;
        }

        public async Task<Tuple<PatientLinkReferenceResponse, ErrorResponse>> LinkPatients(PatientLinkReferenceRequest request)
        {
            var patients = patientRepository.GetPatientInfoWithReferenceNumber(request.Patient.ReferenceNumber);

            return await patients.Map(async (patient) =>
            {
                var requestPatient = request.Patient;

                var programs = (from careContext in requestPatient.CareContexts
                    where patientRepository.GetProgramInfo(requestPatient.ReferenceNumber, careContext.ReferenceNumber)
                        .HasValue
                    select patientRepository.GetProgramInfo(requestPatient.ReferenceNumber, careContext.ReferenceNumber)
                    into careContextPatient
                    select careContextPatient.Map<CareContext>(context =>
                    {
                        var newCareContext = new CareContext
                            {ReferenceNumber = context.ReferenceNumber, Description = context.Description};
                        return newCareContext;
                    })).ToList();

                if (programs.Count != requestPatient.CareContexts.Count())
                {
                    return new Tuple<PatientLinkReferenceResponse, ErrorResponse>
                    (null,
                        new ErrorResponse(new Error(ErrorCode.CareContextNotFound,
                            "Care context not found for given patient")));
                }

                var linkRefNumber = Guid.NewGuid().ToString();

                // method call for generating the OTP
                var session = new Session(linkRefNumber,
                    new Communication(CommunicationMode.MOBILE, patient.PhoneNumber));

                if (await patientVerification.SendTokenFor(session) != null)
                {
                    return new Tuple<PatientLinkReferenceResponse, ErrorResponse>
                        (null, new ErrorResponse(new Error(ErrorCode.OtpInValid, "Unable to create token")));
                }

                var (_, exception) = await linkPatientRepository.SaveLinkPatientDetails(linkRefNumber,
                    requestPatient.ConsentManagerId,
                    requestPatient.ConsentManagerUserId, requestPatient.ReferenceNumber, requestPatient.CareContexts
                        .Select(context => context.ReferenceNumber).ToArray());

                if (exception != null)
                {
                    return new Tuple<PatientLinkReferenceResponse, ErrorResponse>
                    (null,
                        new ErrorResponse(new Error(ErrorCode.CareContextNotFound,
                            "Unable to store data to Database")));
                }

                var expiry = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day,
                        DateTime.Now.Hour, DateTime.Now.Minute + 1, DateTime.Now.Second).ToUniversalTime()
                    .ToString(Constants.DateTimeFormat);

                var meta = new LinkReferenceMeta(nameof(CommunicationMode.MOBILE),
                    patient.PhoneNumber, expiry);
                var patientLinkReferenceResponse = new PatientLinkReferenceResponse(new LinkReference(linkRefNumber,
                    "MEDIATED", meta));

                return new Tuple<PatientLinkReferenceResponse, ErrorResponse>(patientLinkReferenceResponse, null);
            }).ValueOr(
                Task.FromResult(new Tuple<PatientLinkReferenceResponse, ErrorResponse>(null
                    , new ErrorResponse(new Error(ErrorCode.NoPatientFound,"No patient Found"))))
            );
        }

        public async Task<Tuple<PatientLinkResponse, ErrorResponse>> VerifyAndLinkCareContext(HipLibrary.Patient.Models.Request.PatientLinkRequest request)
        {
            if (await patientVerification.Verify(request.LinkReferenceNumber, request.Token) != null)
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
            
            var patientInfo = patientRepository.GetPatientInfoWithReferenceNumber(linkRequest.PatientReferenceNumber);

            return patientInfo.Map(patient =>
            {
                var representations = linkRequest.CareContexts
                    .Where(careContext =>
                        patient.CareContexts.Any(info => info.ReferenceNumber == careContext.CareContextNumber))
                    .Select(context => new CareContextRepresentation(context.CareContextNumber,
                        patient.CareContexts.First(info => info.ReferenceNumber == context.CareContextNumber)
                            .Description));

                var patientLinkResponse = new PatientLinkResponse(new HipLibrary.Patient.Models.Response.LinkPatient(
                    linkRequest.PatientReferenceNumber
                    , patient.FirstName + " " + patient.LastName, representations));
                
                return new Tuple<PatientLinkResponse, ErrorResponse>(patientLinkResponse, null);
            }).ValueOr(new Tuple<PatientLinkResponse, ErrorResponse>(null,
                new ErrorResponse(new Error(ErrorCode.CareContextNotFound, "Care Context Not Found"))));
            
        }
    }
}