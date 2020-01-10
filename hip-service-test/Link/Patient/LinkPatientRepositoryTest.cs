using System;
using System.Collections.Generic;
using FluentAssertions;
using hip_service.Link.Patient;
using hip_service.Link.Patient.Models;
using hip_service_test.Link.Builder;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace hip_service_test.Link.Patient
{
    [Collection("Link Patient Repository Tests")]
    public class LinkPatientRepositoryTest
    {
        public LinkPatientContext GetLinkPatientContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<LinkPatientContext>()
                .UseInMemoryDatabase("hipservice")
                .Options;
            return new LinkPatientContext(optionsBuilder);
        }
        
        [Fact]
        private async void  ShouldReturnSaveLinkRequest()
        {
            var faker = TestBuilder.Faker();
            var dbContext = GetLinkPatientContext();
            var linkPatientRepository = new LinkPatientRepository(dbContext);
            var linkedCareContext = new List<LinkedCareContext> {new LinkedCareContext(faker.Random.Word())};
            var linkRequest = new LinkRequest(faker.Random.Hash(), faker.Random.Hash(), faker.Random.Hash()
                , faker.Random.Hash(), faker.Random.Hash(), linkedCareContext);
            
            var (link, _) = await linkPatientRepository.SaveLinkPatientDetails(linkRequest.LinkReferenceNumber, linkRequest.ConsentManagerId
                ,linkRequest.ConsentManagerUserId, linkRequest.PatientReferenceNumber,
                new[] {(faker.Random.Word())});
            
            link.Should().BeEquivalentTo(linkPatientRepository.GetPatientReferenceNumber(linkRequest.LinkReferenceNumber).Result.Item1);
            dbContext.Database.EnsureDeleted();
        }
        
        [Fact]
        private void ReturnNullUnknownReferenceNumber()
        {
            var faker = TestBuilder.Faker();
            var linkReferenceNumber = faker.Random.Hash() ;
            var dbContext = GetLinkPatientContext();
            var linkPatientRepository = new LinkPatientRepository(dbContext);
            
            linkPatientRepository.GetPatientReferenceNumber(linkReferenceNumber)
                .Result.Item1.Should().BeNull();
            
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
            
            await linkPatientRepository.SaveLinkPatientDetails(linkRequest.LinkReferenceNumber, linkRequest.ConsentManagerId
                ,linkRequest.ConsentManagerUserId, linkRequest.PatientReferenceNumber,
                new[] {(faker.Random.Word())});
            var (_, error) = await linkPatientRepository.SaveLinkPatientDetails(linkRequest.LinkReferenceNumber, linkRequest.ConsentManagerId
                ,linkRequest.ConsentManagerUserId, linkRequest.PatientReferenceNumber,
                new[] {(faker.Random.Word())});

            error.Should().NotBeNull();
            
            dbContext.Database.EnsureDeleted();
        }
    }
}