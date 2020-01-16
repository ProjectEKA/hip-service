using System;
using System.Threading.Tasks;
using hip_service.OTP.Models;

namespace hip_service.OTP
{
    public interface IOtpRepository
    {
        Task<Tuple<OtpRequest, Exception>> Save(string otp, string sessionId);
        
        Task<Tuple<OtpRequest, Exception>> GetOtp(string sessionId);
    }
}