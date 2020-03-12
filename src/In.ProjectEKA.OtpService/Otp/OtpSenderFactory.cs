namespace In.ProjectEKA.OtpService.Otp
{
    using System.Collections.Generic;
    using System.Linq;

    public class OtpSenderFactory
    {
        private readonly IEnumerable<string> whitelistedNumbers;
        private readonly OtpSender otpSender;
        private readonly FakeOtpSender fakeOtpSender;

        public OtpSenderFactory(OtpSender otpSender,
            FakeOtpSender fakeOtpSender,
            IEnumerable<string> whitelistedNumbers)
        {
            this.whitelistedNumbers = whitelistedNumbers ?? new List<string>();
            this.otpSender = otpSender;
            this.fakeOtpSender = fakeOtpSender;
        }

        public IOtpSender ServiceFor(string mobileNumber)
        {
            if (mobileNumber != null && whitelistedNumbers.Any(number => number.Contains(mobileNumber)))
            {
                return fakeOtpSender;
            }

            return otpSender;
        }
    }
}