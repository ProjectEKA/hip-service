
namespace OtpServer.Otp
{
    public interface IOtpWebHandler
    {
        OtpResponse SendOtp(string phoneNumber, string otp);
    }
}