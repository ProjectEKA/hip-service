namespace In.ProjectEKA.DefaultHipTest.Discovery
{
    using static Builder.TestBuilders;
    using DefaultHip.Discovery;
    using DefaultHip.Discovery.Database;
    using DefaultHip.Discovery.Model;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class DiscoveryRequestRepositoryTest
    {
        private static DiscoveryContext GetDiscoveryRequestContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DiscoveryContext>()
                .UseInMemoryDatabase(Faker().Random.String())
                .Options;
            return new DiscoveryContext(optionsBuilder);
        }

        [Fact]
        private async void ShouldAddAndDeleteDiscoveryRequest()
        {
            var dbContext = GetDiscoveryRequestContext();
            var discoveryRequestRepository = new DiscoveryRequestRepository(dbContext);
            const string patientId = "1";
            const string transactionId = "2";
            var discoveryRequest = new DiscoveryRequest(transactionId, patientId);

            var count = await dbContext.DiscoveryRequest.CountAsync();
            count.Should().Be(0);

            await discoveryRequestRepository.Add(discoveryRequest);

            count = await dbContext.DiscoveryRequest.CountAsync();
            count.Should().Be(1);

            await discoveryRequestRepository.Delete(transactionId, patientId);

            count = await dbContext.DiscoveryRequest.CountAsync();
            count.Should().Be(0);

            dbContext.Database.EnsureDeleted();
        }
    }
}