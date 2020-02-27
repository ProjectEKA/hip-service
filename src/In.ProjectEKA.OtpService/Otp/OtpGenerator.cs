namespace In.ProjectEKA.OtpService.Otp
{
    using System;
    
    public class OtpGenerator: IOtpGenerator
    {
        public string GenerateOtp()
        {
            const string chars1 = "1234567890";
            var stringChars1 = new char[6];
            var random1 = new Random();
            for (var i = 0; i < stringChars1.Length; i++)
            {
                stringChars1[i] = chars1[random1.Next(chars1.Length)];
            }
            return new string(stringChars1);
        }
    }
}