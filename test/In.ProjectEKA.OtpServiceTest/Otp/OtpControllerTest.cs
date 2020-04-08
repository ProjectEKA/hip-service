namespace In.ProjectEKA.OtpServiceTest.Otp
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using In.ProjectEKA.OtpService.Otp;
    using Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using Xunit;
    using Optional;
    using OtpService.Clients;
    using OtpService.Common;
    using OtpService.Otp.Model;

    [Collection("Otp Controller Tests")]
    public class OtpControllerTest
    {
        private readonly OtpController otpController;
        private readonly Mock<IOtpRepository> otpRepository;
        private readonly Mock<IOtpGenerator> otpGenerator;
        private readonly Mock<ISmsClient> smsClient;

        public OtpControllerTest()
        {
            otpRepository = new Mock<IOtpRepository>();
            otpGenerator = new Mock<IOtpGenerator>();
            smsClient = new Mock<ISmsClient>();
            var otpService = new OtpSender(otpRepository.Object, otpGenerator.Object, smsClient.Object);
            var otpServiceFactory = new OtpSenderFactory(otpService, new FakeOtpSender(otpRepository.Object), null);
            otpController = new OtpController(otpServiceFactory,
                new OtpVerifier(otpRepository.Object, new OtpProperties(1)));
        }

        [Fact]
        public async Task ShouldSuccessInOtpGeneration()
        {
            var sessionId = TestBuilder.Faker().Random.Hash();
            var otp = TestBuilder.Faker().Random.String();
            var phoneNumber = TestBuilder.Faker().Random.String();
            var otpRequest = new OtpGenerationRequest(sessionId, new Communication("MOBILE", phoneNumber));
            var expectedResult = new Response(ResponseType.Success, "Otp Created");
            otpGenerator.Setup(e => e.GenerateOtp()).Returns(otp);
            smsClient.Setup(e => e.Send(phoneNumber, otp)).ReturnsAsync(expectedResult);
            otpRepository.Setup(e => e.Save(otp, sessionId)).ReturnsAsync(expectedResult);

            var response = await otpController.GenerateOtp(otpRequest);

            otpGenerator.Verify();
            smsClient.Verify();
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
            var otp = TestBuilder.Faker().Random.String();
            var phoneNumber = TestBuilder.Faker().Random.String();
            var otpRequest = new OtpGenerationRequest(TestBuilder.Faker().Random.Hash()
                , new Communication("MOBILE", phoneNumber));
            var expectedResult = new Response(ResponseType.InternalServerError, "OtpGeneration Saving failed");
            otpGenerator.Setup(e => e.GenerateOtp()).Returns(otp);
            smsClient.Setup(e => e.Send(phoneNumber, otp)).ReturnsAsync(expectedResult);

            var response = await otpController.GenerateOtp(otpRequest) as ObjectResult;

            otpGenerator.Verify();
            smsClient.Verify();
            response.Should().NotBeNull()
                .And.BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }


        [Fact]
        public async Task ReturnOtpValidResponse()
        {
            var sessionId = TestBuilder.Faker().Random.Hash();
            var otpToken = TestBuilder.Faker().Random.String();
            var otpRequest = new OtpVerificationRequest(otpToken);
            otpRepository
                .Setup(e => e.GetWith(sessionId))
                .ReturnsAsync(Option.Some(new OtpRequest
                {
                    SessionId = sessionId,
                    RequestedAt = DateTime.Now.ToUniversalTime(),
                    OtpToken = otpToken
                }));

            var response = await otpController.VerifyOtp(sessionId, otpRequest);

            otpRepository.Verify();
            response.Should().NotBeNull()
                .And.Subject.As<OkObjectResult>()
                .Value.Should().BeEquivalentTo(new Response(ResponseType.OtpValid, "Valid OTP"));
        }

        [Fact]
        public async Task ReturnOtpInValidBadRequest()
        {
            var sessionId = TestBuilder.Faker().Random.Hash();
            var otpRequest = new OtpVerificationRequest("1234");
            otpRepository.Setup(e => e.GetWith(sessionId))
                .ReturnsAsync(Option.Some(new OtpRequest {SessionId = sessionId, OtpToken = "random"}));
            var response = await otpController.VerifyOtp(sessionId, otpRequest);
            otpRepository.Verify();
            response.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}