namespace In.ProjectEKA.HipService.Link.Patient
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Transactions;
    using HipLibrary.Patient;
    using HipLibrary.Patient.Model;
    using HipLibrary.Patient.Model.Request;
    using HipLibrary.Patient.Model.Response;
    using Patient = Discovery.Patient.Model.Patient;
    using In.ProjectEKA.HipService.Discovery.Patient;

    public class LinkPatient : ILink
    {
        private readonly ILinkPatientRepository linkPatientRepository;
        private readonly IDiscoveryRequestRepository discoveryRequestRepository;
        private readonly IPatientRepository patientRepository;
        private readonly IPatientVerification patientVerification;
        private readonly IReferenceNumberGenerator referenceNumberGenerator;

        public LinkPatient(ILinkPatientRepository linkPatientRepository,
            IPatientRepository patientRepository,
            IPatientVerification patientVerification,
            IReferenceNumberGenerator referenceNumberGenerator,
            IDiscoveryRequestRepository discoveryRequestRepository)
        {
            this.linkPatientRepository = linkPatientRepository;
            this.patientRepository = patientRepository;
            this.patientVerification = patientVerification;
            this.referenceNumberGenerator = referenceNumberGenerator;
            this.discoveryRequestRepository = discoveryRequestRepository;
        }

        public async Task<Tuple<PatientLinkReferenceResponse, ErrorResponse>> LinkPatients(
            PatientLinkReferenceRequest request)
        {
            var (patient, error) = PatientAndCareContextValidation(request);
            if (error != null) return new Tuple<PatientLinkReferenceResponse, ErrorResponse>(null, error);

            var linkRefNumber = referenceNumberGenerator.NewGuid();
            var session = new Session(linkRefNumber,
                new Communication(CommunicationMode.MOBILE, patient.PhoneNumber));
            if (await patientVerification.SendTokenFor(session) != null)
                return new Tuple<PatientLinkReferenceResponse, ErrorResponse>
                    (null, new ErrorResponse(new Error(ErrorCode.OtpGenerationFailed, "Unable to create token")));

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var (_, exception) = await linkPatientRepository.SaveRequestWith(linkRefNumber,
                request.Patient.ConsentManagerId,
                request.Patient.ConsentManagerUserId, request.Patient.ReferenceNumber, request.Patient.CareContexts
                    .Select(context => context.ReferenceNumber).ToArray()).ConfigureAwait(false);

            if (exception != null)
                return new Tuple<PatientLinkReferenceResponse, ErrorResponse>
                (null, new ErrorResponse(new Error(
                    ErrorCode.ServerInternalError,
                    "Unable to store data to Database")));

            await discoveryRequestRepository.Delete(request.TransactionId, request.Patient.ConsentManagerUserId)
                .ConfigureAwait(false);
            scope.Complete();

            var date = DateTime.Now;
            var time = new TimeSpan(0, 0, 1, 0);
            var expiry = date.Add(time).ToUniversalTime().ToString(Constants.DateTimeFormat);

            var meta = new LinkReferenceMeta(nameof(CommunicationMode.MOBILE),
                patient.PhoneNumber, expiry);
            var patientLinkReferenceResponse = new PatientLinkReferenceResponse(
                new LinkReference(
                    linkRefNumber,
                    "MEDIATED",
                    meta));
            return new Tuple<PatientLinkReferenceResponse, ErrorResponse>(patientLinkReferenceResponse, null);
        }

        public async Task<Tuple<PatientLinkResponse, ErrorResponse>> VerifyAndLinkCareContext(
            HipLibrary.Patient.Model.Request.PatientLinkRequest request)
        {
            var (linkRequest, exception) =
                await linkPatientRepository.GetPatientFor(request.LinkReferenceNumber);

            if (exception != null)
                return new Tuple<PatientLinkResponse, ErrorResponse>
                    (null, new ErrorResponse(new Error(ErrorCode.NoLinkRequestFound, "No request found")));

            if (await patientVerification.Verify(request.LinkReferenceNumber, request.Token) != null)
                return new Tuple<PatientLinkResponse, ErrorResponse>
                    (null, new ErrorResponse(new Error(ErrorCode.OtpInValid, "Otp Invalid")));
            var patientInfo = patientRepository.PatientWith(linkRequest.PatientReferenceNumber);

            return patientInfo.Map(patient =>
            {
                var representations = linkRequest.CareContexts
                    .Where(careContext =>
                        patient.CareContexts.Any(info => info.ReferenceNumber == careContext.CareContextNumber))
                    .Select(context => new CareContextRepresentation(context.CareContextNumber,
                        patient.CareContexts.First(info => info.ReferenceNumber == context.CareContextNumber)
                            .Description));

                var patientLinkResponse = new PatientLinkResponse(new HipLibrary.Patient.Model.Response.LinkPatient(
                    linkRequest.PatientReferenceNumber
                    , patient.FirstName + " " + patient.LastName, representations));

                return new Tuple<PatientLinkResponse, ErrorResponse>(patientLinkResponse, null);
            }).ValueOr(new Tuple<PatientLinkResponse, ErrorResponse>(null,
                new ErrorResponse(new Error(ErrorCode.CareContextNotFound, "Care Context Not Found"))));
        }

        private Tuple<Patient, ErrorResponse> PatientAndCareContextValidation(
            PatientLinkReferenceRequest request)
        {
            return patientRepository.PatientWith(request.Patient.ReferenceNumber).Map(
                patient =>
                {
                    var programs = (from careContext in request.Patient.CareContexts
                        where patientRepository
                            .ProgramInfoWith(request.Patient.ReferenceNumber, careContext.ReferenceNumber)
                            .HasValue
                        select patientRepository.ProgramInfoWith(request.Patient.ReferenceNumber,
                            careContext.ReferenceNumber)
                        into careContextPatient
                        select careContextPatient.Map(context => context)).ToList();
                    if (programs.Count != request.Patient.CareContexts.Count())
                        return new Tuple<Patient, ErrorResponse>
                        (null,
                            new ErrorResponse(new Error(
                                ErrorCode.CareContextNotFound,
                                "Care context not found for given patient")));

                    return new Tuple<Patient, ErrorResponse>(patient, null);
                }).ValueOr(
                new Tuple<Patient, ErrorResponse>(
                    null
                    , new ErrorResponse(new Error(ErrorCode.NoPatientFound, "No patient Found")))
            );
        }
    }
}