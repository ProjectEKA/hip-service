using FluentAssertions;
using In.ProjectEKA.OtpService.Otp;
using In.ProjectEKA.OtpService.Otp.Model;
using In.ProjectEKA.OtpServiceTest.Otp.Builder;
using Moq;
using Optional;
using Xunit;

namespace In.ProjectEKA.OtpServiceTest.Otp
{
    using OtpService.Clients;
    using OtpService.Common;

    [Collection("Otp Service Tests")]
    public class OtpSenderTest
    {
        private readonly OtpSender otpSender;
        private readonly Mock<IOtpRepository> otpRepository = new Mock<IOtpRepository>();
        private readonly Mock<IOtpGenerator> otpGenerator = new Mock<IOtpGenerator>();
        private readonly Mock<ISmsClient> otpWebHandler = new Mock<ISmsClient>();

        public OtpSenderTest()
        {
            otpSender = new OtpSender(otpRepository.Object, otpGenerator.Object, otpWebHandler.Object);
        }

        [Fact]
        private async void ReturnSuccessResponse()
        {
            var sessionId = TestBuilder.Faker().Random.Hash();
            const string otpToken = "123456";
            var phoneNumber = TestBuilder.Faker().Phone.PhoneNumber();
            var testOtpResponse = new Response(ResponseType.Success, "Otp Created");
            var otpRequest = new OtpGenerationRequest(sessionId, new Communication("MOBILE"
                , phoneNumber));
            otpGenerator.Setup(e => e.GenerateOtp()).Returns(otpToken);
            otpWebHandler.Setup(e => e.Send(otpRequest.Communication.Value, otpToken))
                .ReturnsAsync(testOtpResponse);
            otpRepository.Setup(e => e.Save(otpToken, sessionId))
                .ReturnsAsync(testOtpResponse);

            var otpResponse = await otpSender.GenerateOtp(otpRequest);

            otpGenerator.Verify();
            otpWebHandler.Verify();
            otpRepository.Verify();
            otpResponse.Should().BeEquivalentTo(testOtpResponse);
        }
    }
}