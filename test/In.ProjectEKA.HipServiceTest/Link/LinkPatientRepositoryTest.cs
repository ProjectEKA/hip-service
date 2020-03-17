using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace In.ProjectEKA.HipServiceTest.Link
{
    using System;
    using Builder;
    using FluentAssertions;
    using HipService.Link;
    using HipService.Link.Database;
    using HipService.Link.Model;

    [Collection("Link Patient Repository Tests")]
    public class LinkPatientRepositoryTest
    {
        public LinkPatientContext PatientContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<LinkPatientContext>()
                .UseInMemoryDatabase(TestBuilder.Faker().Random.String())
                .Options;
            return new LinkPatientContext(optionsBuilder);
        }

        [Fact]
        private async void ShouldSaveLinkRequest()
        {
            var faker = TestBuilder.Faker();
            var dbContext = PatientContext();
            var linkPatientRepository = new LinkPatientRepository(dbContext);
            var linkReferenceNumber = faker.Random.Hash();
            var (link, _) = await linkPatientRepository.SaveRequestWith(linkReferenceNumber, faker.Random.Hash()
                , faker.Random.Hash(), faker.Random.Hash(),
                new[] {(faker.Random.Word())});
            var (patientFor, _) = await linkPatientRepository.GetPatientFor(linkReferenceNumber);

            link.Should().BeEquivalentTo(patientFor);

            dbContext.Database.EnsureDeleted();
        }

        [Fact]
        private async void ReturnNullUnknownReferenceNumber()
        {
            var faker = TestBuilder.Faker();
            var linkReferenceNumber = faker.Random.Hash();
            var dbContext = PatientContext();
            var linkPatientRepository = new LinkPatientRepository(dbContext);

            var (result, exception) = await linkPatientRepository
                .GetPatientFor(linkReferenceNumber);

            result.Should().BeNull();
            exception.Should().NotBeNull();
            dbContext.Database.EnsureDeleted();
        }

        [Fact]
        private async void ThrowErrorOnSaveOfSamePrimaryKey()
        {
            var faker = TestBuilder.Faker();
            var linkReferenceNumber = faker.Random.Hash();
            var dbContext = PatientContext();
            var linkPatientRepository = new LinkPatientRepository(dbContext);
            var linkedCareContext = new List<LinkedCareContext> {new LinkedCareContext(faker.Random.Word())};
            var linkRequest = new LinkRequest(faker.Random.Hash(),
                linkReferenceNumber,
                faker.Random.Hash(),
                faker.Random.Hash(),
                faker.Random.Hash(),
                linkedCareContext);
            await linkPatientRepository.SaveRequestWith(
                linkRequest.LinkReferenceNumber,
                linkRequest.ConsentManagerId,
                linkRequest.ConsentManagerUserId,
                linkRequest.PatientReferenceNumber,
                new[] {faker.Random.Word()});

            var (_, error) = await linkPatientRepository.SaveRequestWith(
                linkRequest.LinkReferenceNumber,
                linkRequest.ConsentManagerId,
                linkRequest.ConsentManagerUserId,
                linkRequest.PatientReferenceNumber,
                new[] {faker.Random.Word()});

            error.Should().NotBeNull();
            dbContext.Database.EnsureDeleted();
        }

        [Fact]
        private async void ShouldGetLinkedCareContexts()
        {
            var faker = TestBuilder.Faker();
            var dbContext = PatientContext();
            var linkPatientRepository = new LinkPatientRepository(dbContext);
            var consentManagerUserId = faker.Random.Hash();
            var (link, _) = await linkPatientRepository.SaveRequestWith(faker.Random.Hash(), faker.Random.Hash()
                , consentManagerUserId, faker.Random.Hash(),
                new[] {(faker.Random.Word())});
            var (patientFor, _) = await linkPatientRepository.GetLinkedCareContexts(consentManagerUserId);

            link.Should().BeEquivalentTo(patientFor);

            dbContext.Database.EnsureDeleted();
        }
    }
}