namespace In.ProjectEKA.HipService.Link
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Transactions;
    using Common;
    using Discovery;
    using HipLibrary.Patient;
    using HipLibrary.Patient.Model;
    using Logger;
    using Microsoft.Extensions.Options;
    using Model;

    public class LinkPatient
    {
        private readonly IDiscoveryRequestRepository discoveryRequestRepository;
        private readonly ILinkPatientRepository linkPatientRepository;
        private readonly IOptions<OtpServiceConfiguration> otpService;
        private readonly IPatientRepository patientRepository;
        private readonly IPatientVerification patientVerification;
        private readonly ReferenceNumberGenerator referenceNumberGenerator;

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
            var (patient, error) = await PatientAndCareContextValidation(request);
            if (error != null)
            {
                Log.Error(error.Error.Message);
                return (null, error);
            }

            var linkRefNumber = referenceNumberGenerator.NewGuid();
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (!await SaveInitiatedLinkRequest(request.RequestId, request.TransactionId, linkRefNumber)
                    .ConfigureAwait(false))
                    return (null,
                        new ErrorRepresentation(new Error(ErrorCode.DuplicateRequestId, ErrorMessage.DuplicateRequestId))
                        );

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
                    return (null,
                        new ErrorRepresentation(new Error(ErrorCode.ServerInternalError,
                            ErrorMessage.DatabaseStorageError)));

                var session = new Session(
                    linkRefNumber,
                    new Communication(CommunicationMode.MOBILE, patient.PhoneNumber),
                    new OtpGenerationDetail(otpService.Value.SenderSystemName,
                        OtpAction.LINK_PATIENT_CARECONTEXT.ToString()));
                var otpGeneration = await patientVerification.SendTokenFor(session);
                if (otpGeneration != null)
                    return (null,
                        new ErrorRepresentation(new Error(ErrorCode.OtpGenerationFailed, otpGeneration.Message)));

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

        private async Task<ValueTuple<Patient, ErrorRepresentation>> PatientAndCareContextValidation(
            PatientLinkEnquiry request)
        {
            var patient = await patientRepository.PatientWithAsync(request.Patient.ReferenceNumber);
            return patient.Map(patient =>
                    {
                        var programs = request.Patient.CareContexts
                            .Where(careContext =>
                                patient.CareContexts.Any(c => c.ReferenceNumber == careContext.ReferenceNumber))
                            .Select(context => new CareContextRepresentation(context.ReferenceNumber,
                                patient.CareContexts.First(info => info.ReferenceNumber == context.ReferenceNumber)
                                    .Display)).ToList();
                        if (programs.Count != request.Patient.CareContexts.Count())
                            return (null, new ErrorRepresentation(new Error(ErrorCode.CareContextNotFound,
                                ErrorMessage.CareContextNotFound)));

                        return (patient, (ErrorRepresentation) null);
                    })
                .ValueOr((null,
                    new ErrorRepresentation(new Error(ErrorCode.NoPatientFound, ErrorMessage.NoPatientFound))));
        }

        public virtual async Task<ValueTuple<PatientLinkConfirmationRepresentation, string, ErrorRepresentation>>
            VerifyAndLinkCareContext(
            LinkConfirmationRequest request)
        {
            var (linkEnquires, exception) =
                await linkPatientRepository.GetPatientFor(request.LinkReferenceNumber);
            var cmId = "";
            if (exception != null)
                return (null,cmId,
                    new ErrorRepresentation(new Error(ErrorCode.NoLinkRequestFound, ErrorMessage.NoLinkRequestFound)));
            cmId = linkEnquires.ConsentManagerId;

            var errorResponse = await patientVerification.Verify(request.LinkReferenceNumber, request.Token);
            if (errorResponse != null)
                return (null,cmId, new ErrorRepresentation(errorResponse.toError()));

            var patient = await patientRepository.PatientWithAsync(linkEnquires.PatientReferenceNumber);
            return await patient.Map( async patient =>
                {
                    var savedLinkRequests = await linkPatientRepository.Get(request.LinkReferenceNumber);
                    savedLinkRequests.MatchSome(linkRequests =>
                    {
                        foreach (var linkRequest in linkRequests)
                        {
                            linkRequest.Status = true;
                            linkPatientRepository.Update(linkRequest);
                        }
                    });

                    var representations = linkEnquires.CareContexts
                        .Where(careContext =>
                            patient.CareContexts.Any(info => info.ReferenceNumber == careContext.CareContextNumber))
                        .Select(context => new CareContextRepresentation(context.CareContextNumber,
                            patient.CareContexts.First(info => info.ReferenceNumber == context.CareContextNumber)
                                .Display));
                    var patientLinkResponse = new PatientLinkConfirmationRepresentation(
                        new LinkConfirmationRepresentation(
                            linkEnquires.PatientReferenceNumber,
                            $"{patient.Name}",
                            representations));
                    return await SaveLinkedAccounts(linkEnquires)
                        ? (patientLinkResponse,cmId, (ErrorRepresentation) null)
                        : (null,cmId,
                            new ErrorRepresentation(new Error(ErrorCode.NoPatientFound,
                                ErrorMessage.NoPatientFound)));
                }).ValueOr(
                    Task.FromResult<ValueTuple<PatientLinkConfirmationRepresentation, string, ErrorRepresentation>>(
                        (null, cmId,new ErrorRepresentation(new Error(ErrorCode.CareContextNotFound,
                            ErrorMessage.CareContextNotFound)))));
        }

        private async Task<bool> SaveLinkedAccounts(LinkEnquires linkEnquires)
        {
            var linkedAccount = await linkPatientRepository.Save(
                linkEnquires.ConsentManagerUserId,
                linkEnquires.PatientReferenceNumber,
                linkEnquires.LinkReferenceNumber,
                linkEnquires.CareContexts.Select(context => context.CareContextNumber).ToList())
                .ConfigureAwait(false);
            return linkedAccount.HasValue;
        }

        private async Task<bool> SaveInitiatedLinkRequest(string requestId, string transactionId,
            string linkReferenceNumber)
        {
            var savedLinkRequest = await linkPatientRepository.Save(requestId, transactionId, linkReferenceNumber)
                .ConfigureAwait(false);
            return savedLinkRequest.HasValue;
        }
    }
}