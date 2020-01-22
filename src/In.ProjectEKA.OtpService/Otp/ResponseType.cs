namespace In.ProjectEKA.OtpService.Otp
{
    public enum ResponseType
    {
        Success = 1000,
        OtpInvalid,
        OtpGenerationFailed,
        OtpExpired,
        OtpValid,
        InternalServerError,
    }
}