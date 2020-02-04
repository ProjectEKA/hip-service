namespace In.ProjectEKA.HipLibrary.Patient.Model.Response
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
        DiscoveryRequestNotFound
    }
}