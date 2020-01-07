using System;
using System.Threading.Tasks;
using hip_service.OTP.Models;

namespace hip_service.OTP
{
    public interface IOtpRepository
    {
        public Task<Tuple<OtpRequest, Exception>> Save(string otp, string sessionId);
        public Task<Tuple<OtpRequest, Exception>> GetOtp(string sessionId);

    }
}