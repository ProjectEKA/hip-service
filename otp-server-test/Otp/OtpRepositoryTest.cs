using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using otp_server.Otp;
using otp_server.Otp.Models;
using otp_server_test.Otp.Builder;
using Xunit;

namespace otp_server_test.Otp
{
    [Collection("Otp Repository Tests")]
    public class OtpRepositoryTest
    {
        public OtpContext GetOtpContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<OtpContext>()
                .UseInMemoryDatabase("hipservice")
                .Options;
            return new OtpContext(optionsBuilder);
        }

        [Fact]
        private async void SaveOtpGenerationRequest()
        {
            var faker = TestBuilder.Faker();
            var dbContext = GetOtpContext();
            var otpRepository = new OtpRepository(dbContext);
            var sessionId = faker.Random.Hash();
            var otpToken = faker.Random.Number().ToString();
            var testOtpResponse = new OtpResponse(ResponseType.Success, "Otp Created");
            
            var response = await otpRepository.Save(otpToken, sessionId);
            
            response.Should().BeEquivalentTo(testOtpResponse);
            dbContext.Database.EnsureDeleted();
        }

        [Fact]
        private async void ReturnNotNullForValidSessionId()
        {
            var dbContext = GetOtpContext();
            var otpRepository = new OtpRepository(dbContext);
            var sessionId = TestBuilder.Faker().Random.Hash();
            var _ = await otpRepository.Save(TestBuilder.Faker().Random.Number().ToString()
                , sessionId);
            
            var response = await otpRepository.GetOtp(sessionId);
            var otpRequest = response.ValueOr((OtpRequest) null);

            otpRequest.Should().NotBeNull();
            otpRequest.SessionId.Should().BeEquivalentTo(sessionId);
            
            dbContext.Database.EnsureDeleted();
        }

        [Fact]
        private async void ReturnNullForInvalidSessionId()
        {
            var dbContext = GetOtpContext();
            var otpRepository = new OtpRepository(dbContext);
            var _ = await otpRepository.Save(TestBuilder.Faker().Random.Number().ToString()
                , TestBuilder.Faker().Random.Hash());
            
            var response = await otpRepository.GetOtp(TestBuilder.Faker().Random.Hash());
            var otpRequest = response.ValueOr((OtpRequest) null);

            otpRequest.Should().BeNull();
            dbContext.Database.EnsureDeleted();
        }

        [Fact]
        private async void ErrorForSameSessionId()
        {
            var dbContext = GetOtpContext();
            var otpRepository = new OtpRepository(dbContext);
            var sessionId = TestBuilder.Faker().Random.Hash();
            var otpToken = TestBuilder.Faker().Random.Number().ToString();
            var testOtpResponse = new OtpResponse(ResponseType.InternalServerError,"OtpGeneration Saving failed");

            var _ = await otpRepository.Save(otpToken, sessionId);
            var response = await otpRepository.Save(otpToken, sessionId);

            response.Should().BeEquivalentTo(testOtpResponse);
            dbContext.Database.EnsureDeleted();
        }
    }
}