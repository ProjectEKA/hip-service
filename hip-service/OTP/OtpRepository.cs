using System;
using System.Threading.Tasks;
using hip_service.Link.Patient.Models;
using hip_service.OTP.Models;

namespace hip_service.OTP
{
    public class OtpRepository: IOtpRepository
    {
        private readonly LinkPatientContext _linkPatientContext;
        public OtpRepository(LinkPatientContext linkPatientContext)
        {
            _linkPatientContext = linkPatientContext;
        }
        public async Task<Tuple<OtpRequest, Exception>> SaveOtpRequest(string otp, string linkReferenceNumber)
        {
            var dateTimeStamp = new DateTime (DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 
                    DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second).ToUniversalTime().
                ToString("yyyy-MM-ddTHH:mm:ssZ");

            var otpRequest = new OtpRequest(linkReferenceNumber, dateTimeStamp, otp);
            _linkPatientContext.OtpRequests.Add(otpRequest);
            try
            {
                await _linkPatientContext.SaveChangesAsync();
                return new Tuple<OtpRequest, Exception>(otpRequest, null);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return new Tuple<OtpRequest, Exception>(null, exception);
                
            }
        }

        public async Task<Tuple<OtpRequest, Exception>> GetOtp(string linkReferenceNumber)
        {
            try
            {
                var otpRequest = await _linkPatientContext.FindAsync<OtpRequest>(linkReferenceNumber);
                return new Tuple<OtpRequest, Exception>(otpRequest, null);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return new Tuple<OtpRequest, Exception>(null, exception);
                
            }
            
        }
    }
}