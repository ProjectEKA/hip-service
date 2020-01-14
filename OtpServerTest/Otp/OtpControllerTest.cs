using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OtpServer.Otp;
using OtpServerTest.Otp.Builder;
using Xunit;

namespace OtpServerTest.Otp
{
    [Collection("Otp Controller Tests")]
    public class OtpControllerTest
    {
        private readonly OtpController otpController;
        private readonly Mock<IOtpService> otpService;

        public OtpControllerTest()
        {
            otpService = new Mock<IOtpService>();
            otpController = new OtpController(otpService.Object);
        }

        [Fact]
        public async Task ShouldSuccessInOtpGeneration()
        {
            var otpRequest = TestBuilder.otpGenerationRequest()
                .Generate();
            var expectedResult = new OtpResponse(ResponseType.Success,"Otp Created");
            otpService.Setup(e => e.GenerateOtp(It.IsAny<OtpGenerationRequest>())
                ).ReturnsAsync(expectedResult);
            
            var response = await otpController.GenerateOtp(otpRequest);
            
            otpService.Verify();
            response.Should()
                .NotBeNull()
                .And
                .Subject.As<OkObjectResult>()
                .Value
                .Should()
                .BeEquivalentTo(expectedResult);
        }
        
        [Fact]
        public async Task ReturnOtpGenerationBadRequest()
        {
            var otpRequest = TestBuilder.otpGenerationRequest()
                .Generate();
            var expectedResult = new OtpResponse(ResponseType.InternalServerError,"OtpGeneration Saving failed");
            otpService.Setup(e => e.GenerateOtp(It.IsAny<OtpGenerationRequest>())
            ).ReturnsAsync(expectedResult);
            
            var response = await otpController.GenerateOtp(otpRequest);
            
            otpService.Verify();
            response.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Fact]
        public async Task ReturnOtpValidResponse()
        {
            var otpRequest = TestBuilder.otpVerificationRequest()
                .Generate();
            var expectedResult = new OtpResponse(ResponseType.OtpValid,"Valid OTP");
            otpService.Setup(e => e.CheckOtpValue(otpRequest.SessionID,otpRequest.Value)
            ).ReturnsAsync(expectedResult);
            
            var response = await otpController.VerifyOtp(otpRequest);
            
            otpService.Verify();
            response.Should()
                .NotBeNull()
                .And
                .Subject.As<OkObjectResult>()
                .Value
                .Should()
                .BeEquivalentTo(expectedResult);
        }
        
        [Fact]
        public async Task ReturnOtpInValidBadRequest()
        {
            var otpRequest = TestBuilder.otpVerificationRequest()
                .Generate();
            var expectedResult = new OtpResponse(ResponseType.OtpInvalid,"Invalid Otp");
            otpService.Setup(e => e.CheckOtpValue(otpRequest.SessionID,otpRequest.Value)
            ).ReturnsAsync(expectedResult);
            
            var response = await otpController.VerifyOtp(otpRequest);
            
            otpService.Verify();
            response.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}