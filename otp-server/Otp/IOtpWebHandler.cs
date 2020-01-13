using System.Threading.Tasks;

namespace otp_server.Otp
{
    public interface IOtpWebHandler
    {
        OtpResponse SendOtp(string value, string otp);
    }
}