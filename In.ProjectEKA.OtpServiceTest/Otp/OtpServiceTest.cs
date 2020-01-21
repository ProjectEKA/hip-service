using FluentAssertions;
using Moq;
using Optional;
using OtpServer.Otp;
using OtpServer.Otp.Model;
using OtpServerTest.Otp.Builder;
using Xunit;

namespace OtpServerTest.Otp
{
    [Collection("Otp Service Tests")]
    public class OtpServiceTest
    {
        private readonly OtpService otpService;
        private readonly Mock<IOtpRepository> otpRepository = new Mock<IOtpRepository>();
        private readonly Mock<IOtpGenerator> otpGenerator = new Mock<IOtpGenerator>();
        private readonly Mock<IOtpWebHandler> otpWebHandler = new Mock<IOtpWebHandler>();

        public OtpServiceTest()
        {
            otpService = new OtpService(otpRepository.Object, otpGenerator.Object, 
                otpWebHandler.Object);
        }

        [Fact]
        private async void ReturnSuccessResponse()
        {
            var sessionId = TestBuilder.Faker().Random.Hash();
            const string otpToken = "123456";
            var phoneNumber = TestBuilder.Faker().Phone.PhoneNumber();
            var testOtpResponse = new OtpResponse(ResponseType.Success, "Otp Created");
            var otpRequest = new OtpGenerationRequest(sessionId, new Communication("MOBILE"
                ,phoneNumber));
            otpGenerator.Setup(e => e.GenerateOtp()).Returns(otpToken);
            otpWebHandler.Setup(e => e.SendOtp(otpRequest.Communication.Value, otpToken))
                .Returns(testOtpResponse);
            otpRepository.Setup(e =>  e.Save(otpToken, sessionId))
                .ReturnsAsync(testOtpResponse);

            var otpResponse = await otpService.GenerateOtp(otpRequest);
            
            otpGenerator.Verify();
            otpWebHandler.Verify();
            otpRepository.Verify();
            otpResponse.Should().BeEquivalentTo(testOtpResponse);
        }

        [Fact]
        private async void ReturnValidOtpResponse()
        {
            var sessionId = TestBuilder.Faker().Random.Hash();
            var otpToken = TestBuilder.Faker().Random.Number().ToString();
            var testOtpResponse = new OtpResponse(ResponseType.OtpValid,"Valid OTP");
            var testOtpRequest = new OtpRequest(sessionId,It.IsAny<string>(),otpToken);
            otpRepository.Setup(e => e.GetWith(sessionId)).ReturnsAsync(Option.Some(testOtpRequest));

            var otpResponse = await otpService.CheckOtpValue(sessionId, otpToken);
            
            otpResponse.Should().BeEquivalentTo(testOtpResponse);
        }

        [Fact]
        private async void ReturnInvalidOtpResponse()
        {
            var faker = TestBuilder.Faker();
            var sessionId = faker.Random.Hash();
            var testOtpResponse = new OtpResponse(ResponseType.OtpInvalid,"Invalid Otp");
            var testOtpRequest = new OtpRequest(sessionId,It.IsAny<string>()
                ,faker.Random.Number().ToString());
            otpRepository.Setup(e => e.GetWith(sessionId)).ReturnsAsync(Option.Some(testOtpRequest));

            var otpResponse = await otpService.CheckOtpValue(sessionId, faker.Random.Hash());

            otpResponse.Should().BeEquivalentTo(testOtpResponse);
        }
    }
}