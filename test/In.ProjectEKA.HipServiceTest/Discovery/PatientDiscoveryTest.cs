namespace In.ProjectEKA.HipServiceTest.Discovery
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using HipLibrary.Patient;
    using HipLibrary.Patient.Model;
    using HipService.Discovery;
    using Moq;
    using Xunit;
    using Match = HipLibrary.Patient.Model.Match;

    public class PatientDiscoveryTest
    {
        private readonly Mock<IDiscoveryRequestRepository> discoveryRequestRepository =
            new Mock<IDiscoveryRequestRepository>();

        private readonly Mock<IMatchingRepository> matchingRepository = new Mock<IMatchingRepository>();

        [Fact]
        private async void ShouldReturnPatient()
        {
            var patientDiscovery =
                new PatientDiscovery(matchingRepository.Object, discoveryRequestRepository.Object);
            var expectedPatient = new PatientEnquiryRepresentation("1", "John Doee",
                new List<CareContextRepresentation>
                {
                    new CareContextRepresentation("123", "National Cancer program"),
                    new CareContextRepresentation("124", "National TB program")
                }, new List<string>
                {
                    Match.FirstName.ToString(),
                    Match.Gender.ToString()
                });
            var verifiedIdentifiers = new List<Identifier>
            {
                new Identifier(IdentifierType.MOBILE, "+919999999999")
            };
            var unverifiedIdentifiers = new List<Identifier>
            {
                new Identifier(IdentifierType.MR, "123")
            };
            const string patientId = "cm-1";
            var patientRequest = new PatientEnquiry(patientId, verifiedIdentifiers,
                unverifiedIdentifiers, "John", null, Gender.M, new DateTime(2019, 01, 01));
            const string transactionId = "transaction-id-1";
            var discoveryRequest = new DiscoveryRequest(patientRequest, transactionId);
            matchingRepository
                .Setup(repo => repo.Where(discoveryRequest))
                .Returns(Task.FromResult(new List<HipLibrary.Patient.Model.Patient>
                {
                    new HipLibrary.Patient.Model.Patient
                    {
                        Gender = Gender.M.ToString(),
                        Identifier = "1",
                        FirstName = "John",
                        LastName = "Doee",
                        CareContexts = new List<CareContextRepresentation>
                        {
                            new CareContextRepresentation("123", "National Cancer program"),
                            new CareContextRepresentation("124", "National TB program")
                        }
                    }
                }.AsQueryable()));

            var (discoveryResponse, error) = await patientDiscovery.PatientFor(discoveryRequest);

            discoveryResponse.Patient.Should().BeEquivalentTo(expectedPatient);
            discoveryRequestRepository.Verify(
                x => x.Add(It.Is<HipService.Discovery.Model.DiscoveryRequest>(
                    r => r.TransactionId == transactionId
                         && r.ConsentManagerUserId == patientId)), Times.Once);
            error.Should().BeNull();
        }

        [Fact]
        private async void ShouldGetMultiplePatientsFoundError()
        {
            var patientDiscovery = new PatientDiscovery(matchingRepository.Object, discoveryRequestRepository.Object);
            var expectedError =
                new ErrorRepresentation(new Error(ErrorCode.MultiplePatientsFound, "Multiple patients found"));
            var verifiedIdentifiers = new List<Identifier>
            {
                new Identifier(IdentifierType.MOBILE, "+919999999999")
            };
            var patientRequest = new PatientEnquiry("cm-1", verifiedIdentifiers,
                new List<Identifier>(), null, null, Gender.M, new DateTime(2019, 01, 01));
            var discoveryRequest = new DiscoveryRequest(patientRequest, "transaction-id-1");
            matchingRepository
                .Setup(repo => repo.Where(discoveryRequest))
                .Returns(Task.FromResult(new List<HipLibrary.Patient.Model.Patient>
                {
                    new HipLibrary.Patient.Model.Patient(),
                    new HipLibrary.Patient.Model.Patient()
                }.AsQueryable()));

            var (discoveryResponse, error) = await patientDiscovery.PatientFor(discoveryRequest);

            discoveryResponse.Should().BeNull();
            discoveryRequestRepository.Invocations.Count.Should().Be(0);
            error.Should().BeEquivalentTo(expectedError);
        }

        [Fact]
        private async void ShouldGetNoPatientFoundError()
        {
            var patientDiscovery = new PatientDiscovery(matchingRepository.Object, discoveryRequestRepository.Object);
            var expectedError = new ErrorRepresentation(new Error(ErrorCode.NoPatientFound, "No patient found"));
            var verifiedIdentifiers = new List<Identifier>
            {
                new Identifier(IdentifierType.MR, "311231231231")
            };
            var patientRequest = new PatientEnquiry("cm-1", verifiedIdentifiers,
                new List<Identifier>(), null, null,
                Gender.M, new DateTime(2019, 01, 01));
            var discoveryRequest = new DiscoveryRequest(patientRequest, "transaction-id-1");

            var (discoveryResponse, error) = await patientDiscovery.PatientFor(discoveryRequest);

            discoveryResponse.Should().BeNull();
            discoveryRequestRepository.Invocations.Count.Should().Be(0);
            error.Should().BeEquivalentTo(expectedError);
        }
    }
}