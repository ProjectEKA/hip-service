
namespace In.ProjectEKA.OtpService.Otp
{
    public interface IOtpWebHandler
    {
        OtpResponse SendOtp(string phoneNumber, string otp);
    }
}