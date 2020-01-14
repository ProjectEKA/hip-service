using System;
using System.Collections.Generic;
using Bogus;
using FluentAssertions;
using hip_service.Database;
using hip_service.Link.Patient;
using hip_service.Link.Patient.Models;
using hip_service_test.Link.Builder;
using HipLibrary.Patient.Model.Request;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace hip_service_test.Link.Patient
{
    [Collection("Link Patient Repository Tests")]
    public class LinkPatientRepositoryTest
    {
        public DatabaseContext GetLinkPatientContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase("hipservice")
                .Options;
            return new DatabaseContext(optionsBuilder);
        }
        
        [Fact]
        private async void  ShouldSaveLinkRequest()
        {
            var faker = TestBuilder.Faker();
            var dbContext = GetLinkPatientContext();
            var linkPatientRepository = new LinkPatientRepository(dbContext);
            var linkReferenceNumber = faker.Random.Hash();
            var (link, _) = await linkPatientRepository.SaveRequestWith(linkReferenceNumber, faker.Random.Hash()
                ,faker.Random.Hash(), faker.Random.Hash(),
                new[] {(faker.Random.Word())});
            var (patientFor, _) = await linkPatientRepository.GetPatientFor(linkReferenceNumber);

            link.Should().BeEquivalentTo(patientFor);
            
            dbContext.Database.EnsureDeleted();
        }
        
        [Fact]
        private async void ReturnNullUnknownReferenceNumber()
        {
            var faker = TestBuilder.Faker();
            var linkReferenceNumber = faker.Random.Hash() ;
            var dbContext = GetLinkPatientContext();
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
            var linkReferenceNumber = faker.Random.Hash() ;
            var dbContext = GetLinkPatientContext();
            var linkPatientRepository = new LinkPatientRepository(dbContext);
            var linkedCareContext = new List<LinkedCareContext> {new LinkedCareContext(faker.Random.Word())};
            var linkRequest = new LinkRequest(faker.Random.Hash(), linkReferenceNumber, faker.Random.Hash()
                , faker.Random.Hash(), faker.Random.Hash(), linkedCareContext);
            
            await linkPatientRepository.SaveRequestWith(linkRequest.LinkReferenceNumber, linkRequest.ConsentManagerId
                ,linkRequest.ConsentManagerUserId, linkRequest.PatientReferenceNumber,
                new[] {(faker.Random.Word())});
            var (_, error) = await linkPatientRepository.SaveRequestWith(linkRequest.LinkReferenceNumber, linkRequest.ConsentManagerId
                ,linkRequest.ConsentManagerUserId, linkRequest.PatientReferenceNumber,
                new[] {(faker.Random.Word())});

            error.Should().NotBeNull();
            
            dbContext.Database.EnsureDeleted();
        }
    }
}