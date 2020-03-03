using System.Threading.Tasks;
using FluentAssertions;
using In.ProjectEKA.OtpService.Otp;
using In.ProjectEKA.OtpServiceTest.Otp.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace In.ProjectEKA.OtpServiceTest.Otp
{
    using OtpService.Common;

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
            var otpRequest = new OtpGenerationRequest(TestBuilder.Faker().Random.Hash()
                ,new Communication("MOBILE", "+91999999999999"));
            var expectedResult = new Response(ResponseType.Success,"Otp Created");
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
            var otpRequest = new OtpGenerationRequest(TestBuilder.Faker().Random.Hash()
                ,new Communication("MOBILE", "+91999999999999"));
            var expectedResult = new Response(ResponseType.InternalServerError,"OtpGeneration Saving failed");
            otpService.Setup(e => e.GenerateOtp(It.IsAny<OtpGenerationRequest>())
            ).ReturnsAsync(expectedResult);
            
            var response = await otpController.GenerateOtp(otpRequest) as ObjectResult;
            
            otpService.Verify();
            response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }
        
        [Fact]
        public async Task ReturnOtpValidResponse()
        {
            var sessionId = TestBuilder.Faker().Random.Hash();
            var otpRequest = new OtpVerificationRequest("1234");
            var expectedResult = new Response(ResponseType.OtpValid,"Valid OTP");
            otpService.Setup(e => e.CheckOtpValue(sessionId, otpRequest.Value)
            ).ReturnsAsync(expectedResult);
            
            var response = await otpController.VerifyOtp(sessionId, otpRequest);
            
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
            var sessionId = TestBuilder.Faker().Random.Hash();
            var otpRequest = new OtpVerificationRequest("1234");
            var expectedResult = new Response(ResponseType.OtpInvalid,"Invalid Otp");
            otpService.Setup(e => e.CheckOtpValue(sessionId, otpRequest.Value)
            ).ReturnsAsync(expectedResult);
            
            var response = await otpController.VerifyOtp(sessionId, otpRequest);
            
            otpService.Verify();
            response.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}