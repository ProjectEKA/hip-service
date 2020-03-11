namespace In.ProjectEKA.OtpServiceTest.Otp
{
    using System.Threading.Tasks;
    using Builder;
    using FluentAssertions;
    using Moq;
    using OtpService.Common;
    using OtpService.Otp;
    using Xunit;

    public class FakeOtpSenderTest
    {
        [Fact]
        public async Task ShouldSaveOtp()
        {
            var otpRepository = new Mock<IOtpRepository>();
            var sessionId = TestBuilder.Faker().Random.Hash();
            var expectation = new Response(ResponseType.Success, "");
            otpRepository.Setup(e => e.Save("666666", sessionId)).ReturnsAsync(expectation);
            var fakeOtpSender = new FakeOtpSender(otpRepository.Object);

            var result = await fakeOtpSender.GenerateOtp(new OtpGenerationRequest(
                sessionId,
                new Communication(TestBuilder.Faker().Random.String(), TestBuilder.Faker().Random.String())));

            result.Should().BeEquivalentTo(expectation);
        }
    }
}