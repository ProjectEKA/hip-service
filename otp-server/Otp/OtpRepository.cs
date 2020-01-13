using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Optional;
using otp_server.Otp.Models;

namespace otp_server.Otp
{
    public class OtpRepository: IOtpRepository
    {
        private readonly OtpContext otpContext;

        public OtpRepository(OtpContext otpContext)
        {
            this.otpContext = otpContext;
        }
        
        public async Task<OtpResponse> Save(string otp, string sessionId)
        {
            var dateTimeStamp = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            var otpRequest = new OtpRequest(sessionId, dateTimeStamp, otp);
            try
            {
                otpContext.OtpRequests.Add(otpRequest);
                await otpContext.SaveChangesAsync();
                return new OtpResponse(ResponseType.Success,"Otp Created");
            }
            catch (Exception exception)
            {
                return new OtpResponse(ResponseType.InternalServerError,"OtpGeneration Saving failed");
            }
        }
        
        public async Task<Option<OtpRequest>> GetOtp(string sessionId)
        {
            try
            {
                var otpRequest = await otpContext.OtpRequests.FirstAsync(o =>
                    o.SessionId == sessionId);
                return Option.Some(otpRequest);;
            }
            catch (Exception exception)
            {
                return Option.None<OtpRequest>();
            }
        }
    }
}