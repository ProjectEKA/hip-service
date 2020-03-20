using FluentAssertions;
using In.ProjectEKA.OtpService.Otp;
using In.ProjectEKA.OtpService.Otp.Model;
using In.ProjectEKA.OtpServiceTest.Otp.Builder;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace In.ProjectEKA.OtpServiceTest.Otp
{
    using OtpService.Common;

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
            var testOtpResponse = new Response(ResponseType.Success, "Otp Created");
            
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
            await otpRepository.Save(TestBuilder.Faker().Random.Number().ToString()
                , sessionId);
            
            var response = await otpRepository.GetWith(sessionId);
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
            await otpRepository.Save(TestBuilder.Faker().Random.Number().ToString()
                , TestBuilder.Faker().Random.Hash());
            
            var response = await otpRepository.GetWith(TestBuilder.Faker().Random.Hash());
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
            var testOtpResponse = new Response(ResponseType.InternalServerError,"OtpGeneration Saving failed");

            await otpRepository.Save(otpToken, sessionId);
            var response = await otpRepository.Save(otpToken, sessionId);

            response.Should().BeEquivalentTo(testOtpResponse);
            dbContext.Database.EnsureDeleted();
        }
    }
}