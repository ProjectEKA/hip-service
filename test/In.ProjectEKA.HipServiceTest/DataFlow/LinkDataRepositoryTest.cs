namespace In.ProjectEKA.HipServiceTest.DataFlow
{
    using System;
    using System.Linq;
    using Builder;
    using FluentAssertions;
    using HipService.DataFlow;
    using HipService.DataFlow.Database;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class LinkDataRepositoryTest
    {
        private static DataFlowContext DataFlowContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataFlowContext>()
                .UseInMemoryDatabase(TestBuilder.Faker().Random.String())
                .Options;
            return new DataFlowContext(optionsBuilder);
        }

        [Fact]
        private void ShouldAddLinkData()
        {
            var dataFlowContext = DataFlowContext();
            var linkDataRepository = new LinkDataRepository(dataFlowContext);

            dataFlowContext.LinkData.Count().Should().Be(0);
            linkDataRepository.Add(TestBuilder.LinkData("token", DateTime.Now));

            dataFlowContext.LinkData.Count().Should().Be(1);
        }

        [Fact]
        private async void ShouldGetLinkData()
        {
            var dataFlowContext = DataFlowContext();
            var linkDataRepository = new LinkDataRepository(dataFlowContext);
            var linkData = TestBuilder.LinkData("token", DateTime.Now);

            linkDataRepository.Add(linkData);
            var actual = await linkDataRepository.GetAsync(linkData.LinkId);

            actual.Should().BeEquivalentTo(linkData);
        }

        [Fact]
        private async void ShouldGetLinkDataAsNull()
        {
            var dataFlowContext = DataFlowContext();
            var linkDataRepository = new LinkDataRepository(dataFlowContext);

            var actual = await linkDataRepository.GetAsync("1");

            actual.Should().BeNull();
        }
    }
}