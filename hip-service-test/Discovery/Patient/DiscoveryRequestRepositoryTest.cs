using FluentAssertions;
using hip_service.Discovery.Patient;
using hip_service.Discovery.Patient.Database;
using hip_service.Discovery.Patient.Model;
using hip_service_test.Link.Builder;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace hip_service_test.Discovery.Patient
{
    public class DiscoveryRequestRepositoryTest
    {
        private static DiscoveryContext GetDiscoveryRequestContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DiscoveryContext>()
                .UseInMemoryDatabase(TestBuilder.Faker().Random.String())
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