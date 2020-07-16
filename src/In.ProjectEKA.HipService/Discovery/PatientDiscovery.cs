using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using In.ProjectEKA.HipLibrary.Matcher;
using In.ProjectEKA.HipLibrary.Patient;
using In.ProjectEKA.HipLibrary.Patient.Model;
using In.ProjectEKA.HipService.Link;
using In.ProjectEKA.HipService.Link.Model;
using In.ProjectEKA.HipService.Logger;

namespace In.ProjectEKA.HipService.Discovery
{
    public class PatientDiscovery
    {
        private readonly IMatchingRepository matchingRepository;
        private readonly IDiscoveryRequestRepository discoveryRequestRepository;
        private readonly ILinkPatientRepository linkPatientRepository;
        private readonly IPatientRepository patientRepository;

        public PatientDiscovery(
            IMatchingRepository matchingRepository,
            IDiscoveryRequestRepository discoveryRequestRepository,
            ILinkPatientRepository linkPatientRepository,
            IPatientRepository patientRepository)
        {
            this.matchingRepository = matchingRepository;
            this.discoveryRequestRepository = discoveryRequestRepository;
            this.linkPatientRepository = linkPatientRepository;
            this.patientRepository = patientRepository;
        }

        public virtual async Task<ValueTuple<DiscoveryRepresentation, ErrorRepresentation>> PatientFor(
            DiscoveryRequest request)
        {
            if (await AlreadyExists(request.TransactionId))
            {
                return (null,
                    new ErrorRepresentation(new Error(ErrorCode.DuplicateDiscoveryRequest,
                        "Discovery Request already exists")));
            }

            var (linkedAccounts, exception) = await linkPatientRepository.GetLinkedCareContexts(request.Patient.Id);

            if (exception != null)
            {
                Log.Error(exception);
                return (null,
                    new ErrorRepresentation(new Error(ErrorCode.FailedToGetLinkedCareContexts,
                        "Failed to get Linked Care Contexts")));
            }

            var linkedCareContexts = linkedAccounts.ToList();
            if (HasAny(linkedCareContexts))
            {
                return await patientRepository.PatientWith(linkedCareContexts.First().PatientReferenceNumber)
                    .Map(async patient =>
                    {
                        await discoveryRequestRepository.Add(new Model.DiscoveryRequest(request.TransactionId,
                            request.Patient.Id, patient.Identifier));

                        var careContextRepresentations = GetUnlinkedCareContexts(linkedCareContexts, patient).ToList();
                        var maskedPatientEnquiryRepresentation = GetMaskedPatientEnquiryRepresentation(
                            patient.ToPatientEnquiryRepresentation(
                                careContextRepresentations));

                        return (new DiscoveryRepresentation(maskedPatientEnquiryRepresentation),
                            (ErrorRepresentation) null);
                    })
                    .ValueOr(Task.FromResult(((DiscoveryRepresentation) null,
                        new ErrorRepresentation(new Error(ErrorCode.NoPatientFound,
                            ErrorMessage.NoPatientFound)))));
            }

            var patients = await matchingRepository.Where(request);
            var (patientEnquiryRepresentation, error) =
                DiscoveryUseCase.DiscoverPatient(Filter.Do(patients, request).AsQueryable());
            if (patientEnquiryRepresentation == null)
            {
                return (null, error);
            }

            await discoveryRequestRepository.Add(new Model.DiscoveryRequest(request.TransactionId,
                request.Patient.Id, patientEnquiryRepresentation.ReferenceNumber));
            var representation =GetMaskedPatientEnquiryRepresentation(patientEnquiryRepresentation);

            return (new DiscoveryRepresentation(representation), null);
        }

        private async Task<bool> AlreadyExists(string transactionId)
        {
            return await discoveryRequestRepository.RequestExistsFor(transactionId);
        }

        private static bool HasAny(IEnumerable<LinkedAccounts> linkedAccounts)
        {
            return linkedAccounts.Any(account => true);
        }

        private static IEnumerable<CareContextRepresentation> GetUnlinkedCareContexts(
            IEnumerable<LinkedAccounts> linkedAccounts,
            Patient patient)
        {
            var allLinkedCareContexts = linkedAccounts
                .SelectMany(account => account.CareContexts)
                .ToList();

            return patient.CareContexts
                .Where(careContext =>
                    allLinkedCareContexts.Find(linkedCareContext =>
                        linkedCareContext == careContext.ReferenceNumber) == null);
        }
        
        private PatientEnquiryRepresentation GetMaskedPatientEnquiryRepresentation(
            PatientEnquiryRepresentation patient)
        {
            var maskingUtility = new DefaultHip.Patient.MaskingUtility();
            patient.ReferenceNumber = maskingUtility.MaskReference(patient.ReferenceNumber);
            patient.Display = maskingUtility.MaskPatientName(patient.Display);
            foreach (var careContextRepresentation in patient.CareContexts.AsEnumerable())
            {
                careContextRepresentation.ReferenceNumber =
                    new DefaultHip.Patient.MaskingUtility().MaskReference(careContextRepresentation.ReferenceNumber);
                careContextRepresentation.Display =
                    new DefaultHip.Patient.MaskingUtility().MaskCareContextDisplay(careContextRepresentation.Display);
            }

            return patient;
        }
    }
}