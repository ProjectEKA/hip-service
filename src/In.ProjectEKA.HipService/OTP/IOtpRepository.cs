namespace In.ProjectEKA.HipService.OTP
{
    using System;
    using System.Threading.Tasks;
    using Model;

    public interface IOtpRepository
    {
        Task<Tuple<OtpRequest, Exception>> Save(string otp, string sessionId);

        Task<Tuple<OtpRequest, Exception>> GetOtp(string sessionId);
    }
}