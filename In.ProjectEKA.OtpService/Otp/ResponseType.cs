namespace OtpServer.Otp
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