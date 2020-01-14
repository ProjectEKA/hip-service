
namespace OtpServer.Otp
{
    public interface IOtpWebHandler
    {
        OtpResponse SendOtp(string value, string otp);
    }
}