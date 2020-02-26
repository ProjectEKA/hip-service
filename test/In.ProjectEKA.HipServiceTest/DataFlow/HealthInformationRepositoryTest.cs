namespace In.ProjectEKA.HipServiceTest.DataFlow
{
    using System;
    using System.Linq;
    using Builder;
    using FluentAssertions;
    using HipService.DataFlow;
    using HipService.DataFlow.Database;
    using Microsoft.EntityFrameworkCore;
    using Optional;
    using Xunit;

    public class HealthInformationRepositoryTest
    {
        private static DataFlowContext DataFlowContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataFlowContext>()
                .UseInMemoryDatabase(TestBuilder.Faker().Random.String())
                .Options;
            return new DataFlowContext(optionsBuilder);
        }

        [Fact]
        private void ShouldAddHealthInformation()
        {
            var dataFlowContext = DataFlowContext();
            var healthInformationRepository = new HealthInformationRepository(dataFlowContext);

            dataFlowContext.HealthInformation.Count().Should().Be(0);
            healthInformationRepository.Add(TestBuilder.HealthInformation("token", DateTime.Now));

            dataFlowContext.HealthInformation.Count().Should().Be(1);
        }

        [Fact]
        private async void ShouldGetHealthInformation()
        {
            var dataFlowContext = DataFlowContext();
            var healthInformationRepository = new HealthInformationRepository(dataFlowContext);
            var healthInformation = TestBuilder.HealthInformation("token", DateTime.Now);

            healthInformationRepository.Add(healthInformation);
            var actual = await healthInformationRepository.GetAsync(healthInformation.InformationId);

            actual.Should().BeEquivalentTo(Option.Some(healthInformation));
        }

        [Fact]
        private async void ShouldGetHealthInformationAsNull()
        {
            var dataFlowContext = DataFlowContext();
            var healthInformationRepository = new HealthInformationRepository(dataFlowContext);

            var actual = await healthInformationRepository.GetAsync("1");

            actual.HasValue.Should().BeFalse();
        }
    }
}