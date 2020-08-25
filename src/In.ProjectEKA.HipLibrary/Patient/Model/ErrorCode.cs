namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public enum ErrorCode
    {
        NoPatientFound = 3404,
        MultiplePatientsFound = 3403,
        CareContextNotFound = 3402,
        OtpInValid = 3405,
        OtpExpired = 3406,
        OtpGenerationFailed = 3501,
        NoLinkRequestFound = 3413,
        ServerInternalError = 3500,
        DiscoveryRequestNotFound = 3407,
        ContextArtefactIdNotFound = 3416,
        InvalidToken = 3401,
        HealthInformationNotFound = 3502,
        LinkExpired = 3408,
        ExpiredKeyPair = 3410,
        FailedToGetLinkedCareContexts = 3507,
        DuplicateDiscoveryRequest = 3409,
        DuplicateRequestId = 3429,
        CareContextConfiguration = 3430,
        OpenMrsConnection = 3431
    }
}