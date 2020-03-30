namespace In.ProjectEKA.HipService.Link
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Transactions;
    using Discovery;
    using HipLibrary.Patient;
    using HipLibrary.Patient.Model;
    using Logger;
    using Microsoft.Extensions.Options;

    public class LinkPatient
    {
        private readonly ILinkPatientRepository linkPatientRepository;
        private readonly IDiscoveryRequestRepository discoveryRequestRepository;
        private readonly IPatientRepository patientRepository;
        private readonly IPatientVerification patientVerification;
        private readonly ReferenceNumberGenerator referenceNumberGenerator;
        private readonly IOptions<OtpServiceConfiguration> otpService;

        public LinkPatient(
            ILinkPatientRepository linkPatientRepository,
            IPatientRepository patientRepository,
            IPatientVerification patientVerification,
            ReferenceNumberGenerator referenceNumberGenerator,
            IDiscoveryRequestRepository discoveryRequestRepository,
            IOptions<OtpServiceConfiguration> otpService)
        {
            this.linkPatientRepository = linkPatientRepository;
            this.patientRepository = patientRepository;
            this.patientVerification = patientVerification;
            this.referenceNumberGenerator = referenceNumberGenerator;
            this.discoveryRequestRepository = discoveryRequestRepository;
            this.otpService = otpService;
        }

        public virtual async Task<ValueTuple<PatientLinkEnquiryRepresentation, ErrorRepresentation>> LinkPatients(
            PatientLinkEnquiry request)
        {
            var (patient, error) = PatientAndCareContextValidation(request);
            if (error != null)
            {
                Log.Error(error.Error.Message);
                return (null, error);
            }

            var linkRefNumber = referenceNumberGenerator.NewGuid();
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var careContextReferenceNumbers = request.Patient.CareContexts
                    .Select(context => context.ReferenceNumber)
                    .ToArray();
                var (_, exception) = await linkPatientRepository.SaveRequestWith(
                        linkRefNumber,
                        request.Patient.ConsentManagerId,
                        request.Patient.ConsentManagerUserId,
                        request.Patient.ReferenceNumber,
                        careContextReferenceNumbers)
                    .ConfigureAwait(false);
                if (exception != null)
                {
                    return (null,
                        new ErrorRepresentation(new Error(ErrorCode.ServerInternalError,
                            ErrorMessage.DatabaseStorageError)));
                }

                var session = new Session(
                    linkRefNumber,
                    new Communication(CommunicationMode.MOBILE, patient.PhoneNumber));
                var otpGeneration = await patientVerification.SendTokenFor(session);
                if (otpGeneration != null)
                {
                    return (null,
                        new ErrorRepresentation(new Error(ErrorCode.OtpGenerationFailed, otpGeneration.Message)));
                }

                await discoveryRequestRepository.Delete(request.TransactionId, request.Patient.ConsentManagerUserId)
                    .ConfigureAwait(false);
                scope.Complete();
            }

            var time = new TimeSpan(0, 0, otpService.Value.OffsetInMinutes, 0);
            var expiry = DateTime.Now.Add(time).ToUniversalTime().ToString(Constants.DateTimeFormat);
            var meta = new LinkReferenceMeta(nameof(CommunicationMode.MOBILE), patient.PhoneNumber, expiry);
            var patientLinkReferenceResponse = new PatientLinkEnquiryRepresentation(
                new LinkEnquiryRepresentation(linkRefNumber, "MEDIATED", meta));
            return (patientLinkReferenceResponse, null);
        }

        private ValueTuple<Patient, ErrorRepresentation> PatientAndCareContextValidation(
            PatientLinkEnquiry request)
        {
            return patientRepository.PatientWith(request.Patient.ReferenceNumber)
                .Map(
                    patient =>
                    {
                        var programs = request.Patient.CareContexts
                            .Where(careContext =>
                                patient.CareContexts.Any(c => c.ReferenceNumber == careContext.ReferenceNumber))
                            .Select(context => new CareContextRepresentation(context.ReferenceNumber,
                                patient.CareContexts.First(info => info.ReferenceNumber == context.ReferenceNumber)
                                    .Display)).ToList();
                        if (programs.Count != request.Patient.CareContexts.Count())
                        {
                            return (null, new ErrorRepresentation(new Error(ErrorCode.CareContextNotFound,
                                ErrorMessage.CareContextNotFound)));
                        }

                        return (patient, (ErrorRepresentation) null);
                    })
                .ValueOr((null,
                    new ErrorRepresentation(new Error(ErrorCode.NoPatientFound, ErrorMessage.NoPatientFound))));
        }

        public virtual async Task<ValueTuple<PatientLinkConfirmationRepresentation, ErrorRepresentation>>
            VerifyAndLinkCareContext(
                LinkConfirmationRequest request)
        {
            var verifyOtp = await patientVerification.Verify(request.LinkReferenceNumber, request.Token);
            if (verifyOtp != null)
            {
                return (null, new ErrorRepresentation(new Error(ErrorCode.OtpInValid, verifyOtp.Message)));
            }

            var (linkRequest, exception) =
                await linkPatientRepository.GetPatientFor(request.LinkReferenceNumber);

            if (exception != null)
            {
                return (null,
                    new ErrorRepresentation(new Error(ErrorCode.NoLinkRequestFound, ErrorMessage.NoLinkRequestFound)));
            }

            return patientRepository.PatientWith(linkRequest.PatientReferenceNumber)
                .Map(
                    patient =>
                    {
                        var representations = linkRequest.CareContexts
                            .Where(careContext =>
                                patient.CareContexts.Any(info => info.ReferenceNumber == careContext.CareContextNumber))
                            .Select(context => new CareContextRepresentation(context.CareContextNumber,
                                patient.CareContexts.First(info => info.ReferenceNumber == context.CareContextNumber)
                                    .Display));
                        var patientLinkResponse = new PatientLinkConfirmationRepresentation(
                            new LinkConfirmationRepresentation(
                                linkRequest.PatientReferenceNumber,
                                $"{patient.FirstName} {patient.LastName}",
                                representations));
                        return (patientLinkResponse, (ErrorRepresentation) null);
                    })
                .ValueOr((null,
                    new ErrorRepresentation(new Error(ErrorCode.CareContextNotFound,
                        ErrorMessage.CareContextNotFound))));
        }
    }
}