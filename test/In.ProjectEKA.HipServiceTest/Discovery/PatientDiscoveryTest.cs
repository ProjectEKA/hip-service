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
    using HipService.Link;
    using HipService.Link.Model;
    using Moq;
    using Optional;
    using Xunit;
    using Match = HipLibrary.Patient.Model.Match;
    using TestBuilders = Link.Builder.TestBuilders;

    public class PatientDiscoveryTest
    {
        private readonly Patient testPatient =
            new Patient
            {
                PhoneNumber = "+91666666666666",
                Identifier = "1",
                Gender = TestBuilders.Faker().PickRandom<Gender>(),
                Name = "John",
                CareContexts = new List<CareContextRepresentation>
                {
                    new CareContextRepresentation("123", "National Cancer program"),
                    new CareContextRepresentation("124", "National TB program")
                }
            };

        private readonly Mock<IDiscoveryRequestRepository> discoveryRequestRepository =
            new Mock<IDiscoveryRequestRepository>();

        private readonly Mock<ILinkPatientRepository> linkPatientRepository = new Mock<ILinkPatientRepository>();
        private readonly Mock<IMatchingRepository> matchingRepository = new Mock<IMatchingRepository>();
        private readonly Mock<IPatientRepository> patientRepository = new Mock<IPatientRepository>();

        [Fact]
        private async void ShouldReturnPatient()
        {
            var patientDiscovery = new PatientDiscovery(
                matchingRepository.Object,
                discoveryRequestRepository.Object,
                linkPatientRepository.Object,
                patientRepository.Object);
            var expectedPatient = new PatientEnquiryRepresentation("1", "John",
                new List<CareContextRepresentation>
                {
                    new CareContextRepresentation("124", "National TB program")
                }, new List<string>
                {
                    Match.ConsentManagerUserId.ToString()
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
            const string transactionId = "transaction-id-1";
            var patientRequest = new PatientEnquiry(patientId, verifiedIdentifiers,
                unverifiedIdentifiers, "John", Gender.M, 2019);
            var discoveryRequest = new DiscoveryRequest(patientRequest, transactionId);
            var sessionId = TestBuilders.Faker().Random.Hash();
            ICollection<string> linkedCareContext = new[] {"123"};
            var testLinkAccounts = new LinkedAccounts("1", sessionId, TestBuilders.Faker().Random.Hash()
                , It.IsAny<string>(), linkedCareContext.ToList());
            linkPatientRepository.Setup(e => e.GetLinkedCareContexts(patientId))
                .ReturnsAsync(new Tuple<IEnumerable<LinkedAccounts>, Exception>(
                    new List<LinkedAccounts> {testLinkAccounts},
                    null));
            patientRepository.Setup(x => x.PatientWith(testPatient.Identifier))
                .Returns(Option.Some(testPatient));
            matchingRepository
                .Setup(repo => repo.Where(discoveryRequest))
                .Returns(Task.FromResult(new List<Patient>
                {
                    new Patient
                    {
                        Gender = Gender.M,
                        Identifier = "1",
                        Name = "John",
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
            var patientDiscovery = new PatientDiscovery(
                matchingRepository.Object,
                discoveryRequestRepository.Object,
                linkPatientRepository.Object,
                patientRepository.Object);
            var expectedError =
                new ErrorRepresentation(new Error(ErrorCode.MultiplePatientsFound, "Multiple patients found"));
            var verifiedIdentifiers = new List<Identifier>
            {
                new Identifier(IdentifierType.MOBILE, "+919999999999")
            };
            const string patientId = "cm-1";
            var dateOfBirth = new DateTime(2019, 01, 01);
            var patientRequest = new PatientEnquiry(patientId,
                verifiedIdentifiers,
                new List<Identifier>(),
                null,
                Gender.M,
                (ushort) dateOfBirth.Year);
            var discoveryRequest = new DiscoveryRequest(patientRequest, "transaction-id-1");
            linkPatientRepository.Setup(e => e.GetLinkedCareContexts(patientId))
                .ReturnsAsync(new Tuple<IEnumerable<LinkedAccounts>, Exception>(new List<LinkedAccounts>(), null));

            matchingRepository
                .Setup(repo => repo.Where(discoveryRequest))
                .Returns(Task.FromResult(new List<Patient>
                {
                    new Patient {YearOfBirth = (ushort) dateOfBirth.Year},
                    new Patient {YearOfBirth = (ushort) dateOfBirth.Year}
                }.AsQueryable()));

            var (discoveryResponse, error) = await patientDiscovery.PatientFor(discoveryRequest);
            discoveryResponse.Should().BeNull();
            error.Should().BeEquivalentTo(expectedError);
        }

        [Fact]
        private async void ShouldGetNoPatientFoundError()
        {
            var patientDiscovery = new PatientDiscovery(
                matchingRepository.Object,
                discoveryRequestRepository.Object,
                linkPatientRepository.Object,
                patientRepository.Object);
            var expectedError = new ErrorRepresentation(new Error(ErrorCode.NoPatientFound, "No patient found"));
            var verifiedIdentifiers = new List<Identifier>
            {
                new Identifier(IdentifierType.MR, "311231231231")
            };
            const string patientId = "cm-1";
            var patientRequest = new PatientEnquiry("cm-1", verifiedIdentifiers,
                new List<Identifier>(), null,
                Gender.M, 2019);
            var discoveryRequest = new DiscoveryRequest(patientRequest, "transaction-id-1");
            linkPatientRepository.Setup(e => e.GetLinkedCareContexts(patientId))
                .ReturnsAsync(new Tuple<IEnumerable<LinkedAccounts>, Exception>(new List<LinkedAccounts>(), null));

            var (discoveryResponse, error) = await patientDiscovery.PatientFor(discoveryRequest);

            discoveryResponse.Should().BeNull();
            error.Should().BeEquivalentTo(expectedError);
        }

        [Fact]
        private async void ShouldReturnAnErrorWhenDiscoveryRequestAlreadyExists()
        {
            var patientDiscovery = new PatientDiscovery(
                matchingRepository.Object,
                discoveryRequestRepository.Object,
                linkPatientRepository.Object,
                patientRepository.Object);
            var expectedError =
                new ErrorRepresentation(new Error(ErrorCode.DuplicateDiscoveryRequest, "Request already exists"));
            var transactionId = Builder.TestBuilders.RandomString();
            var discoveryRequest = new DiscoveryRequest(null, transactionId);
            discoveryRequestRepository.Setup(repository => repository.RequestExistsFor(transactionId))
                .ReturnsAsync(true);

            var (discoveryResponse, error) = await patientDiscovery.PatientFor(discoveryRequest);

            discoveryResponse.Should().BeNull();
            error.Should().BeEquivalentTo(expectedError);
        }
    }
}