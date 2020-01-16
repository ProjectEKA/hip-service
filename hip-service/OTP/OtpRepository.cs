using System;
using System.Threading.Tasks;
using hip_service.Link.Patient;
using hip_service.Link.Patient.Models;
using hip_service.OTP.Models;
using Microsoft.EntityFrameworkCore;

namespace hip_service.OTP
{
    public class OtpRepository: IOtpRepository
    {
        private readonly LinkPatientContext linkPatientContext;
        public OtpRepository(LinkPatientContext linkPatientContext)
        {
            this.linkPatientContext = linkPatientContext;
        }
        public async Task<Tuple<OtpRequest, Exception>> Save(string otp, string sessionId)
        {
            var dateTimeStamp = DateTime.Now.ToUniversalTime().ToString(Constants.DateTimeFormat);
            var otpRequest = new OtpRequest(sessionId, dateTimeStamp, otp);
            linkPatientContext.OtpRequests.Add(otpRequest);
            try
            {
                await linkPatientContext.SaveChangesAsync();
                return new Tuple<OtpRequest, Exception>(otpRequest, null);
            }
            catch (Exception exception)
            {
                return new Tuple<OtpRequest, Exception>(null, exception);
            }
        }

        public async Task<Tuple<OtpRequest, Exception>> GetOtp(string sessionId)
        {
            try
            {
                var otpRequest = await linkPatientContext.OtpRequests.FirstAsync(o =>
                    o.SessionId == sessionId);
                return new Tuple<OtpRequest, Exception>(otpRequest, null);
            }
            catch (Exception exception)
            {
                return new Tuple<OtpRequest, Exception>(null, exception);
            }
        }
    }
}