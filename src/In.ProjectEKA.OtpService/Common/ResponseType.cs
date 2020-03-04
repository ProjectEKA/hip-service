namespace In.ProjectEKA.OtpService.Common
{
    public enum ResponseType
    {
        Success = 1000,
        InternalServerError,
        OtpInvalid,
        OtpExpired,
        OtpValid,
    }
}