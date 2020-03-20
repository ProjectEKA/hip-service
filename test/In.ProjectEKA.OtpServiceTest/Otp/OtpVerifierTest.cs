namespace In.ProjectEKA.OtpServiceTest.Otp
{
    using Builder;
    using FluentAssertions;
    using Moq;
    using Optional;
    using OtpService.Common;
    using OtpService.Otp;
    using OtpService.Otp.Model;
    using Xunit;

    public class OtpVerifierTest
    {
        private readonly OtpVerifier otpService;
        private readonly Mock<IOtpRepository> otpRepository = new Mock<IOtpRepository>();

        public OtpVerifierTest()
        {
            otpService = new OtpVerifier(otpRepository.Object);
        }


        [Fact]
        private async void ReturnValidOtpResponse()
        {
            var sessionId = TestBuilder.Faker().Random.Hash();
            var otpToken = TestBuilder.Faker().Random.Number().ToString();
            var testOtpResponse = new Response(ResponseType.OtpValid, "Valid OTP");
            var testOtpRequest = new OtpRequest(sessionId, It.IsAny<string>(), otpToken);
            otpRepository.Setup(e => e.GetWith(sessionId)).ReturnsAsync(Option.Some(testOtpRequest));

            var otpResponse = await otpService.CheckOtpValue(sessionId, otpToken);

            otpResponse.Should().BeEquivalentTo(testOtpResponse);
        }

        [Fact]
        private async void ReturnInvalidOtpResponse()
        {
            var faker = TestBuilder.Faker();
            var sessionId = faker.Random.Hash();
            var testOtpResponse = new Response(ResponseType.OtpInvalid, "Invalid Otp");
            var testOtpRequest = new OtpRequest(sessionId, It.IsAny<string>()
                , faker.Random.Number().ToString());
            otpRepository.Setup(e => e.GetWith(sessionId)).ReturnsAsync(Option.Some(testOtpRequest));

            var otpResponse = await otpService.CheckOtpValue(sessionId, faker.Random.Hash());

            otpResponse.Should().BeEquivalentTo(testOtpResponse);
        }
    }
}