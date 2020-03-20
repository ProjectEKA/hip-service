namespace In.ProjectEKA.OtpServiceTest.Otp
{
    using System.Collections.Generic;
    using FluentAssertions;
    using OtpService.Otp;
    using Xunit;

    public class OtpSenderFactoryTest
    {
        [Theory]
        [InlineData("9999999999")]
        [InlineData("+91-9999999999")]
        public void ShouldReturnFakeOtpSender(string mobileNumber)
        {
            var fakeOtpSender = new FakeOtpSender(null);
            var otpSenderFactory = new OtpSenderFactory(
                new OtpSender(null, null, null),
                fakeOtpSender,
                new List<string>
                {
                    "+91-9999999999"
                });

            var otpSender = otpSenderFactory.ServiceFor(mobileNumber);

            otpSender.As<FakeOtpSender>().Should().NotBeNull().And.BeEquivalentTo(fakeOtpSender);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("anything")]
        [InlineData("+91-2222222222")]
        public void ShouldReturnDefaultOtpSender(string mobileNumber)
        {
            var otpSender = new OtpSender(null, null, null);
            var otpSenderFactory = new OtpSenderFactory(
                otpSender,
                new FakeOtpSender(null),
                new List<string>
                {
                    "+91-9999999999"
                });

            var response = otpSenderFactory.ServiceFor(mobileNumber);

            response.As<OtpSender>().Should().NotBeNull().And.BeEquivalentTo(otpSender);
        }
    }
}