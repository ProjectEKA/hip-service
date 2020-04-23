namespace In.ProjectEKA.HipServiceTest.DataFlow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Builder;
    using FluentAssertions;
    using HipLibrary.Patient.Model;
    using HipService.Common;
    using HipService.DataFlow;
    using HipService.DataFlow.Encryptor;
    using Hl7.Fhir.Model;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Moq;
    using Optional;
    using Org.BouncyCastle.Crypto;
    using Xunit;

    public class DataEntryFactoryTest
    {
        private static Mock<IEncryptor> encryptor = new Mock<IEncryptor>();

        [Fact]
        private void ShouldGetComponentEntry()
        {
            var dataFlowConfiguration = Options.Create(
                new DataFlowConfiguration {DataSizeLimitInMbs = 5, DataLinkTtlInMinutes = 5});
            var hipConfiguration = Options.Create(
                new HipConfiguration {Url = "https://hip/"});
            var serviceScopeFactory = new Mock<IServiceScopeFactory>(MockBehavior.Strict);
            var dataEntryFactory = new DataEntryFactory(
                serviceScopeFactory.Object,
                dataFlowConfiguration,
                hipConfiguration, encryptor.Object);
            var dataEntries = new Entries(new List<Bundle> {new Bundle()});
            var expectedEntries = new List<Entry>
            {
                new Entry(
                    "5zGyp5O9GkggioxwWyUGOQ==",
                    "application/fhir+json",
                    "MD5",
                    null)
            }.AsEnumerable();
            var transactionId = TestBuilder.Faker().Random.Uuid().ToString();
            var keyMaterial = TestBuilder.KeyMaterialLib();
            encryptor.Setup(e => e.EncryptData(keyMaterial,
                It.IsAny<AsymmetricCipherKeyPair>(),
                It.IsAny<string>(),
                It.IsAny<string>())).Returns(Option.Some("5zGyp5O9GkggioxwWyUGOQ=="));

            var entries = dataEntryFactory.Process(dataEntries, keyMaterial, transactionId);

            entries.HasValue.Should().BeTrue();
            entries.Map(e => e.Entries.Should().BeEquivalentTo(expectedEntries));
        }

        [Fact]
        private void ShouldGetLinkEntry()
        {
            var dataFlowConfiguration = Options.Create(
                new DataFlowConfiguration {DataSizeLimitInMbs = 0, DataLinkTtlInMinutes = 5});
            var hipConfiguration = Options.Create(
                new HipConfiguration {Url = "https://hip"});
            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            var serviceScope = new Mock<IServiceScope>();
            var serviceProvider = new Mock<IServiceProvider>();
            var healthInformationRepository = new Mock<IHealthInformationRepository>();
            var dataEntryFactory = new DataEntryFactory(
                serviceScopeFactory.Object,
                dataFlowConfiguration,
                hipConfiguration,
                encryptor.Object);

            serviceScopeFactory.Setup(x => x.CreateScope()).Returns(serviceScope.Object);
            serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);
            serviceProvider.Setup(x => x.GetService(typeof(IHealthInformationRepository)))
                .Returns(healthInformationRepository.Object);
            var keyMaterialLib = TestBuilder.KeyMaterialLib();
            var transactionId = TestBuilder.Faker().Random.Uuid().ToString();
            encryptor.Setup(e => e.EncryptData(keyMaterialLib,
                It.IsAny<AsymmetricCipherKeyPair>(),
                It.IsAny<string>(),
                It.IsAny<string>())).Returns(Option.Some("https://hip/health-information"));
            var entries = dataEntryFactory.Process(
                new Entries(new List<Bundle> {new Bundle()}), keyMaterialLib, transactionId);

            entries.HasValue.Should().BeTrue();
            entries.MatchSome(dataEntries =>
            {
                foreach (var entry in dataEntries.Entries)
                {
                    entry.Content.Should().BeNull();
                    entry.Link.Should().Contain("https://hip/health-information");
                }
            });
        }
    }
}