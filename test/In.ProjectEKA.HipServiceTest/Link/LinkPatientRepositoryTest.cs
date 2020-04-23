using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace In.ProjectEKA.HipServiceTest.Link
{
    using System;
    using System.Linq;
    using Builder;
    using FluentAssertions;
    using HipService.Link;
    using HipService.Link.Database;
    using HipService.Link.Model;

    [Collection("Link Patient Repository Tests")]
    public class LinkPatientRepositoryTest
    {
        private static LinkPatientContext PatientContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<LinkPatientContext>()
                .UseInMemoryDatabase(TestBuilders.Faker().Random.String())
                .Options;
            return new LinkPatientContext(optionsBuilder);
        }

        [Fact]
        private async void ShouldSaveLinkRequest()
        {
            var faker = TestBuilders.Faker();
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
            var faker = TestBuilders.Faker();
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
        private async void ThrowErrorOnSaveOfSamePrimaryKeyLinkEnquires()
        {
            var faker = TestBuilders.Faker();
            var linkReferenceNumber = faker.Random.Hash();
            var dbContext = PatientContext();
            var linkPatientRepository = new LinkPatientRepository(dbContext);
            var linkedCareContext = new List<CareContext> {new CareContext(faker.Random.Word())};
            var linkRequest = new LinkEnquires(faker.Random.Hash(),
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
        private async void ShouldSaveLinkedAccounts()
        {
            var faker = TestBuilders.Faker();
            var dbContext = PatientContext();
            var linkPatientRepository = new LinkPatientRepository(dbContext);
            var consentManagerUserId = faker.Random.Hash();
            var link = await linkPatientRepository.Save(consentManagerUserId, faker.Random.Hash()
                , faker.Random.Hash(), new[] {faker.Random.Word()});
            var (patientFor, _) = await linkPatientRepository.GetLinkedCareContexts(consentManagerUserId);

            link.MatchSome(l => l.Should().BeEquivalentTo(patientFor.First()));

            dbContext.Database.EnsureDeleted();
        }

        [Fact]
        private async void ThrowErrorOnSaveOfSamePrimaryKeyLinkedAccounts()
        {
            var faker = TestBuilders.Faker();
            var dbContext = PatientContext();
            var linkPatientRepository = new LinkPatientRepository(dbContext);
            var consentManagerUserId = faker.Random.Hash();
            var linkReferenceNumber = faker.Random.Hash();
            await linkPatientRepository.Save(consentManagerUserId, faker.Random.Hash()
                , linkReferenceNumber, new[] {faker.Random.Word()});
            var linkedAccount = await linkPatientRepository.Save(consentManagerUserId, faker.Random.Hash()
                , linkReferenceNumber, new[] {faker.Random.Word()});

            linkedAccount.HasValue.Should().BeFalse();

            dbContext.Database.EnsureDeleted();
        }

        [Fact]
        private async void ShouldGetLinkedCareContexts()
        {
            var faker = TestBuilders.Faker();
            var dbContext = PatientContext();
            var linkPatientRepository = new LinkPatientRepository(dbContext);
            var consentManagerUserId = faker.Random.Hash();

            var linkedAccounts = await linkPatientRepository.Save(consentManagerUserId,
                faker.Random.Hash(),
                faker.Random.Hash(),
                new[] {faker.Random.Word()});
            var (patientFor, _) = await linkPatientRepository.GetLinkedCareContexts(consentManagerUserId);

            linkedAccounts.MatchSome(l => { l.Should().BeEquivalentTo(patientFor.First()); });

            dbContext.Database.EnsureDeleted();
        }

        [Fact]
        private async void ShouldSaveInitiatedLinkRequest()
        {
            var faker = TestBuilders.Faker();
            var dbContext = PatientContext();
            var linkPatientRepository = new LinkPatientRepository(dbContext);
            var hashValue = faker.Random.Hash();
            var request = await linkPatientRepository.Save(hashValue,
                hashValue,
                hashValue);

            request.MatchSome(l => l.RequestId.Should().BeEquivalentTo(hashValue));
            request.MatchSome(l => l.TransactionId.Should().BeEquivalentTo(hashValue));
            request.MatchSome(l => l.LinkReferenceNumber.Should().BeEquivalentTo(hashValue));

            dbContext.Database.EnsureDeleted();
        }

        [Fact]
        private async void ThrowErrorOnSaveOfSamePrimaryKeyInitiatedLinkRequest()
        {
            var faker = TestBuilders.Faker();
            var dbContext = PatientContext();
            var linkPatientRepository = new LinkPatientRepository(dbContext);
            var hashValue = faker.Random.Hash();
            await linkPatientRepository.Save(hashValue, hashValue, hashValue);
            var request = await linkPatientRepository.Save(hashValue,
                hashValue,
                hashValue);

            request.HasValue.Should().BeFalse();

            dbContext.Database.EnsureDeleted();
        }
    }
}