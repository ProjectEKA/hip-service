namespace In.ProjectEKA.HipService.OTP.Model
{
    using System.ComponentModel.DataAnnotations;

    public class OtpRequest
    {
        public OtpRequest()
        {
        }

        public OtpRequest(string sessionId, string dateTimeStamp, string otpToken)
        {
            SessionId = sessionId;
            DateTimeStamp = dateTimeStamp;
            OtpToken = otpToken;
        }

        [Key] public string SessionId { get; set; }

        public string DateTimeStamp { get; set; }

        public string OtpToken { get; set; }
    }
}