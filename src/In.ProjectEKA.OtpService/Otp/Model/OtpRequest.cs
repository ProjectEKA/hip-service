namespace In.ProjectEKA.OtpService.Otp.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class OtpRequest
    {
        [Key] public string SessionId { get; set; }

        public DateTime RequestedAt { get; set; }

        public string OtpToken { get; set; }
    }
}