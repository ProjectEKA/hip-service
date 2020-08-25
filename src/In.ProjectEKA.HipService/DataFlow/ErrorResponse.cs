namespace In.ProjectEKA.HipService.DataFlow
{
    using HipLibrary.Patient.Model;

    public static class ErrorResponse
    {
        public static readonly Error InvalidToken = new Error(ErrorCode.InvalidToken, ErrorMessage.InvalidToken);
        public static readonly Error LinkExpired = new Error(ErrorCode.LinkExpired, ErrorMessage.LinkExpired);

        public static readonly Error HealthInformationNotFound =
            new Error(ErrorCode.HealthInformationNotFound, ErrorMessage.HealthInformationNotFound);
    }
}