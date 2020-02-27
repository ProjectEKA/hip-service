namespace In.ProjectEKA.HipServiceTest.DataFlow
{
    using FluentAssertions;
    using In.ProjectEKA.HipService.DataFlow;
    using In.ProjectEKA.HipService.DataFlow.Database;
    using Builder;
    using HipService.DataFlow.Model;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    [Collection("Data Flow Repository Tests")]
    public class DataFlowRepositoryTest
    {
        private static DataFlowContext DataFlowContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataFlowContext>()
                .UseInMemoryDatabase(TestBuilder.Faker().Random.String())
                .Options;
            return new DataFlowContext(optionsBuilder);
        }

        [Fact]
        private async void ShouldStoreDataFlowRequest()
        {
            var faker = TestBuilder.Faker();
            var transactionId = faker.Random.Hash();
            var request = TestBuilder.HealthInformationRequest(transactionId);
            var dbContext = DataFlowContext();
            var dataFlowRepository = new DataFlowRepository(dbContext);

            var result = await dataFlowRepository.SaveRequest(transactionId, request);

            result.HasValue.Should().BeFalse();
            result.Map(r => r.Should().BeNull());

            dbContext.Database.EnsureDeleted();
        }

        [Fact]
        private async void ThrowErrorOnSaveOfSamePrimaryKey()
        {
            var faker = TestBuilder.Faker();
            var request = TestBuilder.HealthInformationRequest(faker.Random.Hash());
            var dbContext = DataFlowContext();
            var dataFlowRepository = new DataFlowRepository(dbContext);

            await dataFlowRepository.SaveRequest(request.TransactionId, request);
            var result = await dataFlowRepository.SaveRequest(request.TransactionId, request);

            result.HasValue.Should().BeTrue();
            result.Map(r => r.Should().NotBeNull());

            dbContext.Database.EnsureDeleted();
        }
    }
}