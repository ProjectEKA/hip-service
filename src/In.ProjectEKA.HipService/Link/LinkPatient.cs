namespace In.ProjectEKA.HipService.Link
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Transactions;
    using Discovery;
    using HipLibrary.Patient;
    using HipLibrary.Patient.Model;
    using Hl7.Fhir.Model;
    using Logger;
    using Microsoft.Extensions.Options;
    using Model;
    using Patient = HipLibrary.Patient.Model.Patient;
    using Task = System.Threading.Tasks.Task;

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
                if (!await SaveInitiatedLinkRequest(request.RequestId, request.TransactionId, linkRefNumber)
                    .ConfigureAwait(false))
                {
                    return (null,
                        new ErrorRepresentation(new Error(ErrorCode.DuplicateRequestId, ErrorMessage.DuplicateRequestId))); 
                }

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
            var errorResponse = await patientVerification.Verify(request.LinkReferenceNumber, request.Token);
            if (errorResponse != null)
            {
                return (null, new ErrorRepresentation(errorResponse.toError()));
            }

            var (linkEnquires, exception) =
                await linkPatientRepository.GetPatientFor(request.LinkReferenceNumber);

            if (exception != null)
            {
                return (null,
                    new ErrorRepresentation(new Error(ErrorCode.NoLinkRequestFound, ErrorMessage.NoLinkRequestFound)));
            }

            return await patientRepository.PatientWith(linkEnquires.PatientReferenceNumber)
                .Map( async patient =>
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
                        ? (patientLinkResponse, (ErrorRepresentation) null)
                        : (null,
                            new ErrorRepresentation(new Error(ErrorCode.NoPatientFound,
                                ErrorMessage.NoPatientFound)));
                }).ValueOr(Task.FromResult<ValueTuple<PatientLinkConfirmationRepresentation, ErrorRepresentation>>((null, new ErrorRepresentation(new Error(ErrorCode.CareContextNotFound,
                        ErrorMessage.CareContextNotFound)))) );
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

        private async Task<bool> SaveInitiatedLinkRequest(string requestId, string transactionId, string linkReferenceNumber)
        {
            var savedLinkRequest = await linkPatientRepository.Save(requestId, transactionId, linkReferenceNumber)
                .ConfigureAwait(false);
            return savedLinkRequest.HasValue;
        }
    }
}