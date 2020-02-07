namespace In.ProjectEKA.OtpService.Otp.Model
{
    using System.ComponentModel.DataAnnotations;

    public class OtpRequest
    {
        [Key]
        public string SessionId { get; set; }
        public string DateTimeStamp { get; set; }
        public string OtpToken { get; set; }

        public OtpRequest(string sessionId, string dateTimeStamp, string otpToken)
        {
            SessionId = sessionId;
            DateTimeStamp = dateTimeStamp;
            OtpToken = otpToken;
        }
    }
}