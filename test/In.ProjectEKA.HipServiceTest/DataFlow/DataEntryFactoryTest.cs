namespace In.ProjectEKA.HipServiceTest.DataFlow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using HipLibrary.Patient.Model;
    using HipService.Common;
    using HipService.DataFlow;
    using Hl7.Fhir.Model;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Moq;
    using Optional;
    using Xunit;

    public class DataEntryFactoryTest
    {
        [Fact]
        private void ShouldGetComponentEntry()
        {
            var dataFlowConfiguration = Options.Create(
                new DataFlowConfiguration {DataSizeLimitInMbs = 5, DataLinkTTLInMinutes = 5});
            var hipConfiguration = Options.Create(
                new HipConfiguration {Url = "https://hip/"});
            var serviceScopeFactory = new Mock<IServiceScopeFactory>(MockBehavior.Strict);
            var dataEntryFactory = new DataEntryFactory(
                serviceScopeFactory.Object,
                dataFlowConfiguration,
                hipConfiguration);
            var dataEntries = new Entries(new List<Bundle> {new Bundle()});
            var expectedEntries = new List<Entry>
            {
                new Entry(
                    "{\"resourceType\":\"Bundle\"}",
                    "application/fhir+json",
                    "MD5",
                    null)
            }.AsEnumerable();

            var entries = dataEntryFactory.Process(Option.Some(dataEntries));

            entries.HasValue.Should().BeTrue();
            entries.Map(e => e.Should().BeEquivalentTo(expectedEntries));
        }

        [Fact]
        private void ShouldGetLinkEntry()
        {
            var dataFlowConfiguration = Options.Create(
                new DataFlowConfiguration {DataSizeLimitInMbs = 0, DataLinkTTLInMinutes = 5});
            var hipConfiguration = Options.Create(
                new HipConfiguration {Url = "https://hip"});
            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            var serviceScope = new Mock<IServiceScope>();
            var serviceProvider = new Mock<IServiceProvider>();
            var healthInformationRepository = new Mock<IHealthInformationRepository>();
            var dataEntryFactory = new DataEntryFactory(
                serviceScopeFactory.Object,
                dataFlowConfiguration,
                hipConfiguration);

            serviceScopeFactory.Setup(x => x.CreateScope()).Returns(serviceScope.Object);
            serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);
            serviceProvider.Setup(x => x.GetService(typeof(IHealthInformationRepository)))
                .Returns(healthInformationRepository.Object);
            var entries = dataEntryFactory.Process(
                Option.Some(new Entries(new List<Bundle> {new Bundle()})));

            entries.HasValue.Should().BeTrue();
            entries.MatchSome(dataEntries =>
            {
                foreach (var entry in dataEntries)
                {
                    entry.Content.Should().BeNull();
                    entry.Link.Href.Should().Contain("https://hip/health-information");
                }
            });
        }
    }
}