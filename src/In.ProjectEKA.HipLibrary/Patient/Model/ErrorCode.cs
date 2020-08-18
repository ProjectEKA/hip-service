namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public enum ErrorCode
    {
        NoPatientFound = 1000,
        MultiplePatientsFound = 1001,
        CareContextNotFound = 1002,
        OtpInValid = 1003,
        OtpExpired = 1004,
        OtpGenerationFailed = 1005,
        NoLinkRequestFound = 1006,
        ServerInternalError = 1007,
        DiscoveryRequestNotFound = 1008,
        ContextArtefactIdNotFound = 1009,
        InvalidToken = 1010,
        HealthInformationNotFound = 1011,
        LinkExpired = 1012,
        ExpiredKeyPair = 1013,
        FailedToGetLinkedCareContexts = 1014,
        DuplicateDiscoveryRequest = 1015,
        DuplicateRequestId = 1016,
        CareContextConfiguration = 1017
    }
}