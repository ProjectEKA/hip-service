namespace In.ProjectEKA.OtpService.Otp
{
    using System;
    using System.Threading.Tasks;
    using Model;
    using Microsoft.EntityFrameworkCore;
    using Optional;
    using Logger;
    
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
                Log.Fatal(exception, exception.StackTrace);
                return new OtpResponse(ResponseType.InternalServerError,"OtpGeneration Saving failed");
            }
        }
        
        public async Task<Option<OtpRequest>> GetWith(string sessionId)
        {
            try
            {
                var otpRequest = await otpContext.OtpRequests.FirstAsync(o => 
                    o.SessionId == sessionId);
                return Option.Some(otpRequest);
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, exception.StackTrace);
                return Option.None<OtpRequest>();
            }
        }
    }
}