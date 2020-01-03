using System;
using System.Threading.Tasks;
using hip_service.OTP.Models;

namespace hip_service.OTP
{
    public interface IOtpRepository
    {
        public Task<Tuple<OtpRequest, Exception>> SaveOtpRequest(string otp, string linkReferenceNumber);
        public Task<Tuple<OtpRequest, Exception>> GetOtp(string linkReferenceNumber);

    }
}