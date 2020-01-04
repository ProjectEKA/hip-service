using System.ComponentModel.DataAnnotations;

namespace hip_service.OTP.Models
{
    public class OtpRequest
    {
        [Key]
        public string SessionId { get; set; }

        public string DateTimeStamp { get; set; }
        
        public string OtpToken { get; set; }

        public OtpRequest()
        {
        }

        public OtpRequest(string sessionId, string dateTimeStamp, string otpToken)
        {
            SessionId = sessionId;
            DateTimeStamp = dateTimeStamp;
            OtpToken = otpToken;
        }
    }
}