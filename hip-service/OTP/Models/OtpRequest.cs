using System.ComponentModel.DataAnnotations;

namespace hip_service.OTP.Models
{
    public class OtpRequest
    {
        [Key]
        public string LinkReferenceNumber { get; set; }

        public string DateTimeStamp { get; set; }
        
        public string OtpToken { get; set; }

        public OtpRequest()
        {
        }

        public OtpRequest(string linkReferenceNumber, string dateTimeStamp, string otpToken)
        {
            LinkReferenceNumber = linkReferenceNumber;
            DateTimeStamp = dateTimeStamp;
            OtpToken = otpToken;
        }
    }
}