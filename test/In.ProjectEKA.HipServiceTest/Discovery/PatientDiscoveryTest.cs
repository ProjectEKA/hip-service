namespace In.ProjectEKA.HipServiceTest.Discovery
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Builder;
    using FluentAssertions;
    using HipLibrary.Patient;
    using HipLibrary.Patient.Model;
    using HipService.Discovery;
    using HipService.Link;
    using HipService.Link.Model;
    using Link.Builder;
    using Moq;
    using Optional;
    using Xunit;
    using Match = HipLibrary.Patient.Model.Match;

    public class PatientDiscoveryTest
    {
        private readonly Patient testPatient =
            new Patient
            {
                PhoneNumber = "+91666666666666",
                Identifier = "1",
                FirstName = "John",
                LastName = "Doee",
                Gender = TestBuilder.Faker().Random.Word(),
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
            var expectedPatient = new PatientEnquiryRepresentation("1", "John Doee",
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
            var patientRequest = new PatientEnquiry(patientId, verifiedIdentifiers,
                unverifiedIdentifiers, "John", null, Gender.M, new DateTime(2019, 01, 01));
            const string transactionId = "transaction-id-1";
            var discoveryRequest = new DiscoveryRequest(patientRequest, transactionId);

            var sessionId = TestBuilder.Faker().Random.Hash();
            ICollection<LinkedCareContext> linkedCareContext = new[] {new LinkedCareContext("123")};
            var testLinkRequest = new LinkRequest("1", sessionId,
                TestBuilder.Faker().Random.Hash(), TestBuilder.Faker().Random.Hash()
                , It.IsAny<string>(), linkedCareContext);
            var linkRequests = new List<LinkRequest> {testLinkRequest};

            linkPatientRepository.Setup(e => e.GetLinkedCareContexts(patientId))
                .ReturnsAsync((linkRequests, null));
            patientRepository.Setup(x => x.PatientWith(testPatient.Identifier))
                .Returns(Option.Some(testPatient));
            matchingRepository
                .Setup(repo => repo.Where(discoveryRequest))
                .Returns(Task.FromResult(new List<Patient>
                {
                    new Patient
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
            var patientRequest = new PatientEnquiry(patientId, verifiedIdentifiers,
                new List<Identifier>(), null, null, Gender.M, new DateTime(2019, 01, 01));
            var discoveryRequest = new DiscoveryRequest(patientRequest, "transaction-id-1");
            linkPatientRepository.Setup(e => e.GetLinkedCareContexts(patientId))
                .ReturnsAsync((new List<LinkRequest>(), null));
            matchingRepository
                .Setup(repo => repo.Where(discoveryRequest))
                .Returns(Task.FromResult(new List<Patient>
                {
                    new Patient(),
                    new Patient()
                }.AsQueryable()));

            var (discoveryResponse, error) = await patientDiscovery.PatientFor(discoveryRequest);

            discoveryResponse.Should().BeNull();
            discoveryRequestRepository.Invocations.Count.Should().Be(1);
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
                new List<Identifier>(), null, null,
                Gender.M, new DateTime(2019, 01, 01));
            var discoveryRequest = new DiscoveryRequest(patientRequest, "transaction-id-1");
            linkPatientRepository.Setup(e => e.GetLinkedCareContexts(patientId))
                .ReturnsAsync((new List<LinkRequest>(), null));

            var (discoveryResponse, error) = await patientDiscovery.PatientFor(discoveryRequest);

            discoveryResponse.Should().BeNull();
            discoveryRequestRepository.Invocations.Count.Should().Be(1);
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
            var transactionId = TestBuilders.RandomString();
            var discoveryRequest = new DiscoveryRequest(null, transactionId);
            discoveryRequestRepository.Setup(repository => repository.RequestExistsFor(transactionId))
                .ReturnsAsync(true);

            var (discoveryResponse, error) = await patientDiscovery.PatientFor(discoveryRequest);

            discoveryResponse.Should().BeNull();
            error.Should().BeEquivalentTo(expectedError);
        }
        
        [Fact]
        private async void ShouldReturnPatientMatchingMobileAndIdentifier()
        {
            var patientHina =
                new Patient
                {
                    PhoneNumber = "+91-8888888888",
                    Identifier = "RVH1003",
                    FirstName = "Hina",
                    LastName = "Patel",
                    Gender = TestBuilder.Faker().Random.Word(),
                    CareContexts = new List<CareContextRepresentation>
                    {
                        new CareContextRepresentation("NCP1008", "National Cancer program"),
                        new CareContextRepresentation("BI-KTH-12.05.0024", "National TB program")
                    }
                };
            
            var patientDiscovery = new PatientDiscovery(
                matchingRepository.Object,
                discoveryRequestRepository.Object,
                linkPatientRepository.Object,
                patientRepository.Object);
            var expectedPatient = new PatientEnquiryRepresentation("RVH1003", "Hina Patel",
                new List<CareContextRepresentation>
                {
                    new CareContextRepresentation("NCP1008", "National Cancer program"),
                    new CareContextRepresentation("BI-KTH-12.05.0024", "National TB program")
                }, new List<string>
                {
                    "FirstName", "LastName", "Gender", "MR"
                });
            var verifiedIdentifiers = new List<Identifier>
            {
                new Identifier(IdentifierType.MOBILE, "+91-8888888888")
            };
            var unverifiedIdentifiers = new List<Identifier>
            {
                new Identifier(IdentifierType.MR, "RVH1003")
            };
            const string patientId = "cm-2";
            var patientRequest = new PatientEnquiry(patientId, verifiedIdentifiers,
                unverifiedIdentifiers, "Hina", "Patel", Gender.F, new DateTime(2019, 01, 01));
            const string transactionId = "transaction-id-1";
            var discoveryRequest = new DiscoveryRequest(patientRequest, transactionId);

            var sessionId = TestBuilder.Faker().Random.Hash();
            ICollection<LinkedCareContext> linkedCareContext = new[] {new LinkedCareContext("NCP1008")};
            patientRepository.Setup(x => x.PatientWith(patientHina.Identifier))
                .Returns(Option.Some(patientHina));
            matchingRepository
                .Setup(repo => repo.Where(discoveryRequest))
                .Returns(Task.FromResult(new List<Patient>
                {
                    new Patient
                    {
                        Gender = Gender.F.ToString(),
                        Identifier = "RVH1003",
                        FirstName = "Hina",
                        LastName = "Patel",
                        CareContexts = new List<CareContextRepresentation>
                        {
                            new CareContextRepresentation("NCP1008", "National Cancer program"),
                            new CareContextRepresentation("BI-KTH-12.05.0024", "National TB program")
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
    }
}