namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public enum ErrorCode
    {
        NoPatientFound = 1000,
        MultiplePatientsFound,
        CareContextNotFound,
        OtpInValid,
        OtpExpired,
        OtpGenerationFailed,
        NoLinkRequestFound,
        ServerInternalError,
        DiscoveryRequestNotFound,
        ContextArtefactIdNotFound,
        InvalidToken,
        HealthInformationNotFound,
        LinkExpired,
        ExpiredKeyPair,
        FailedToGetLinkedCareContexts,
        DuplicateDiscoveryRequest,
        DuplicateRequestId
    }
}