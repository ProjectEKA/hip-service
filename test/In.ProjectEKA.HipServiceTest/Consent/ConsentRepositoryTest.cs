namespace In.ProjectEKA.HipServiceTest.Consent
{
    using System.Linq;
    using Builder;
    using FluentAssertions;
    using HipService.Consent;
    using HipService.Consent.Database;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class ConsentRepositoryTest
    {
        private static ConsentContext ConsentContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ConsentContext>()
                .UseInMemoryDatabase(TestBuilder.Faker().Random.String())
                .Options;
            return new ConsentContext(optionsBuilder);
        }

        [Fact]
        async void ShouldStoreConsent()
        {
            var consentContext = ConsentContext();
            var consentRepository = new ConsentRepository(consentContext);
            consentContext.ConsentArtefact.Count().Should().Be(0);

            await consentRepository.AddAsync(TestBuilder.Consent());

            consentContext.ConsentArtefact.Count().Should().Be(1);
        }

        [Fact]
        async void ShouldFetchConsent()
        {
            var consentContext = ConsentContext();
            var consentRepository = new ConsentRepository(consentContext);
            var consentAdded = TestBuilder.Consent();
            await consentRepository.AddAsync(consentAdded);

            var consentFetched = await consentRepository.GetFor(consentAdded.ConsentArtefactId);
            
            consentFetched.Should().BeEquivalentTo(consentAdded);
        }

        [Fact]
        async void ShouldReturnNullWhenConsentIsNotFound()
        {
            var consentContext = ConsentContext();
            var consentRepository = new ConsentRepository(consentContext);
            var consentAdded = TestBuilder.Consent();
            await consentRepository.AddAsync(consentAdded);

            var consentFetched = await consentRepository.GetFor("not-found-consent-id");
            
            consentFetched.Should().BeNull();
        }
    }
}