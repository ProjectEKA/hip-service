namespace In.ProjectEKA.HipServiceTest.Discovery
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using HipLibrary.Matcher;
    using HipLibrary.Patient;
    using HipLibrary.Patient.Model;
    using HipService.Discovery;
    using HipService.Link;
    using HipService.Link.Model;
    using Moq;
    using Optional;
    using Xunit;
    using Match = HipLibrary.Patient.Model.Match;
    using static Builder.TestBuilders;

    public class PatientDiscoveryTest
    {
        private readonly Mock<IDiscoveryRequestRepository> discoveryRequestRepository =
            new Mock<IDiscoveryRequestRepository>();

        private readonly Mock<ILinkPatientRepository> linkPatientRepository = new Mock<ILinkPatientRepository>();

        private readonly Mock<IMatchingRepository> matchingRepository = new Mock<IMatchingRepository>();

        private readonly Mock<IPatientRepository> patientRepository = new Mock<IPatientRepository>();

        [Fact]
        private async void ShouldReturnPatientForAlreadyLinkedPatient()
        {
            var patientDiscovery = new PatientDiscovery(
                matchingRepository.Object,
                discoveryRequestRepository.Object,
                linkPatientRepository.Object,
                patientRepository.Object);
            var phoneNumber = Faker().Phone.PhoneNumber();
            var verifiedIdentifiers = new[] {new Identifier(IdentifierType.MOBILE, phoneNumber)};
            var unverifiedIdentifiers = new[] {new Identifier(IdentifierType.MR, Faker().Random.String())};
            var patientId = Faker().Random.String();
            var name = Faker().Name.FullName();
            var alreadyLinked =
                new CareContextRepresentation(Faker().Random.Uuid().ToString(), Faker().Random.String());
            var unlinkedCareContext =
                new CareContextRepresentation(Faker().Random.Uuid().ToString(), Faker().Random.String());
            var expectedPatient = new PatientEnquiryRepresentation(
                patientId,
                name,
                new[] {unlinkedCareContext},
                new[] {Match.ConsentManagerUserId.ToString()});
            var transactionId = Faker().Random.String();
            var patientRequest = new PatientEnquiry(patientId,
                verifiedIdentifiers,
                unverifiedIdentifiers,
                name,
                Gender.M,
                2019);
            var discoveryRequest = new DiscoveryRequest(patientRequest, RandomString(), transactionId, DateTime.Now);
            var sessionId = Faker().Random.Hash();
            var linkedCareContext = new[] {alreadyLinked.ReferenceNumber};
            var testLinkAccounts = new LinkedAccounts(patientId,
                sessionId,
                Faker().Random.Hash(),
                It.IsAny<string>(),
                linkedCareContext.ToList());
            var testPatient =
                new Patient
                {
                    PhoneNumber = phoneNumber,
                    Identifier = patientId,
                    Gender = Faker().PickRandom<Gender>(),
                    Name = name,
                    CareContexts = new[]
                    {
                        alreadyLinked,
                        unlinkedCareContext
                    }
                };
            linkPatientRepository.Setup(e => e.GetLinkedCareContexts(patientId))
                .ReturnsAsync(new Tuple<IEnumerable<LinkedAccounts>, Exception>(
                    new List<LinkedAccounts> {testLinkAccounts},
                    null));
            patientRepository.Setup(x => x.PatientWith(testPatient.Identifier))
                .Returns(Option.Some(testPatient));

            var (discoveryResponse, error) = await patientDiscovery.PatientFor(discoveryRequest);

            discoveryResponse.Patient.Should().BeEquivalentTo(expectedPatient);
            discoveryRequestRepository.Verify(
                x => x.Add(It.Is<HipService.Discovery.Model.DiscoveryRequest>(
                    r => r.TransactionId == transactionId && r.ConsentManagerUserId == patientId)),
                Times.Once);
            error.Should().BeNull();
        }

        [Fact]
        private async void ShouldReturnAPatientWhichIsNotLinkedAtAll()
        {
            var patientDiscovery = new PatientDiscovery(
                matchingRepository.Object,
                discoveryRequestRepository.Object,
                linkPatientRepository.Object,
                patientRepository.Object);
            var referenceNumber = Faker().Random.String();
            var name = Faker().Random.String();
            var phoneNumber = Faker().Phone.PhoneNumber();
            var consentManagerUserId = Faker().Random.String();
            var transactionId = Faker().Random.String();
            const ushort yearOfBirth = 2019;
            var careContextRepresentations = new[]
            {
                new CareContextRepresentation(Faker().Random.String(), Faker().Random.String()),
                new CareContextRepresentation(Faker().Random.String(), Faker().Random.String())
            };
            var expectedPatient = new PatientEnquiryRepresentation(referenceNumber,
                name,
                careContextRepresentations,
                new List<string>
                {
                    Match.Mobile.ToString(),
                    Match.Name.ToString(),
                    Match.Gender.ToString(),
                    Match.Mr.ToString()
                });
            var verifiedIdentifiers = new List<Identifier>
            {
                new Identifier(IdentifierType.MOBILE, phoneNumber)
            };
            var unverifiedIdentifiers = new List<Identifier>
            {
                new Identifier(IdentifierType.MR, referenceNumber)
            };
            var patientRequest = new PatientEnquiry(consentManagerUserId,
                verifiedIdentifiers,
                unverifiedIdentifiers,
                name,
                Gender.M,
                yearOfBirth);
            var discoveryRequest = new DiscoveryRequest(patientRequest, RandomString(),transactionId, DateTime.Now);
            linkPatientRepository.Setup(e => e.GetLinkedCareContexts(consentManagerUserId))
                .ReturnsAsync(new Tuple<IEnumerable<LinkedAccounts>, Exception>(new List<LinkedAccounts>(), null));
            matchingRepository
                .Setup(repo => repo.Where(discoveryRequest))
                .Returns(Task.FromResult(new List<Patient>
                {
                    new Patient
                    {
                        Gender = Gender.M,
                        Identifier = referenceNumber,
                        Name = name,
                        CareContexts = careContextRepresentations,
                        PhoneNumber = phoneNumber,
                        YearOfBirth = yearOfBirth
                    }
                }.AsQueryable()));

            var (discoveryResponse, error) = await patientDiscovery.PatientFor(discoveryRequest);

            discoveryResponse.Patient.Should().BeEquivalentTo(expectedPatient);
            discoveryRequestRepository.Verify(
                x => x.Add(It.Is<HipService.Discovery.Model.DiscoveryRequest>(
                    r => r.TransactionId == transactionId && r.ConsentManagerUserId == consentManagerUserId)),
                Times.Once);
            error.Should().BeNull();
        }

        [Fact]
        private async void ShouldReturnAPatientWhenUnverifiedIdentifierIsNull()
        {
            var patientDiscovery = new PatientDiscovery(
                matchingRepository.Object,
                discoveryRequestRepository.Object,
                linkPatientRepository.Object,
                patientRepository.Object);
            var referenceNumber = Faker().Random.String();
            var consentManagerUserId = Faker().Random.String();
            var transactionId = Faker().Random.String();
            var name = Faker().Name.FullName();
            const ushort yearOfBirth = 2019;
            var phoneNumber = Faker().Phone.PhoneNumber();
            var careContextRepresentations = new[]
            {
                new CareContextRepresentation(Faker().Random.String(), Faker().Random.String()),
                new CareContextRepresentation(Faker().Random.String(), Faker().Random.String())
            };
            var expectedPatient = new PatientEnquiryRepresentation(
                referenceNumber,
                name,
                careContextRepresentations,
                new List<string>
                {
                    Match.Mobile.ToString(),
                    Match.Name.ToString(),
                    Match.Gender.ToString()
                });
            var verifiedIdentifiers = new[] {new Identifier(IdentifierType.MOBILE, phoneNumber)};
            var patientRequest = new PatientEnquiry(consentManagerUserId,
                verifiedIdentifiers,
                null,
                name,
                Gender.M,
                yearOfBirth);
            var discoveryRequest = new DiscoveryRequest(patientRequest, RandomString(),transactionId, DateTime.Now);
            linkPatientRepository.Setup(e => e.GetLinkedCareContexts(consentManagerUserId))
                .ReturnsAsync(new Tuple<IEnumerable<LinkedAccounts>, Exception>(new List<LinkedAccounts>(), null));
            matchingRepository
                .Setup(repo => repo.Where(discoveryRequest))
                .Returns(Task.FromResult(new List<Patient>
                {
                    new Patient
                    {
                        Gender = Gender.M,
                        Identifier = referenceNumber,
                        Name = name,
                        CareContexts = careContextRepresentations,
                        PhoneNumber = phoneNumber,
                        YearOfBirth = yearOfBirth
                    }
                }.AsQueryable()));

            var (discoveryResponse, error) = await patientDiscovery.PatientFor(discoveryRequest);

            discoveryResponse.Patient.Should().BeEquivalentTo(expectedPatient);
            discoveryRequestRepository.Verify(
                x => x.Add(It.Is<HipService.Discovery.Model.DiscoveryRequest>(
                    r => r.TransactionId == transactionId && r.ConsentManagerUserId == consentManagerUserId)),
                Times.Once);
            error.Should().BeNull();
        }

        [Theory]
        [ClassData(typeof(EmptyIdentifierTestData))]
        private async void ReturnMultiplePatientsErrorWhenUnverifiedIdentifierIs(IEnumerable<Identifier> identifiers)
        {
            var patientDiscovery = new PatientDiscovery(
                matchingRepository.Object,
                discoveryRequestRepository.Object,
                linkPatientRepository.Object,
                patientRepository.Object);
            var expectedError =
                new ErrorRepresentation(new Error(ErrorCode.MultiplePatientsFound, "Multiple patients found"));
            var verifiedIdentifiers = new[] {new Identifier(IdentifierType.MOBILE, Faker().Phone.PhoneNumber())};
            var consentManagerUserId = Faker().Random.String();
            const ushort yearOfBirth = 2019;
            var gender = Faker().PickRandom<Gender>();
            var name = Faker().Name.FullName();
            var patientRequest = new PatientEnquiry(consentManagerUserId,
                verifiedIdentifiers,
                identifiers,
                name,
                gender,
                yearOfBirth);
            var discoveryRequest = new DiscoveryRequest(patientRequest, Faker().Random.String(), RandomString(), DateTime.Now);
            linkPatientRepository.Setup(e => e.GetLinkedCareContexts(consentManagerUserId))
                .ReturnsAsync(new Tuple<IEnumerable<LinkedAccounts>, Exception>(new List<LinkedAccounts>(), null));

            matchingRepository
                .Setup(repo => repo.Where(discoveryRequest))
                .Returns(Task.FromResult(new List<Patient>
                {
                    new Patient
                    {
                        YearOfBirth = yearOfBirth,
                        Gender = gender,
                        Name = name
                    },
                    new Patient
                    {
                        YearOfBirth = yearOfBirth,
                        Gender = gender,
                        Name = name
                    }
                }.AsQueryable()));

            var (discoveryResponse, error) = await patientDiscovery.PatientFor(discoveryRequest);

            discoveryResponse.Should().BeNull();
            error.Should().BeEquivalentTo(expectedError);
        }

        [Fact]
        private async void ShouldGetMultiplePatientsFoundErrorWhenSameUnverifiedIdentifiersAlsoMatch()
        {
            var patientDiscovery = new PatientDiscovery(
                matchingRepository.Object,
                discoveryRequestRepository.Object,
                linkPatientRepository.Object,
                patientRepository.Object);
            var expectedError =
                new ErrorRepresentation(new Error(ErrorCode.MultiplePatientsFound, "Multiple patients found"));
            var patientReferenceNumber = Faker().Random.String();
            var consentManagerUserId = Faker().Random.String();
            const ushort yearOfBirth = 2019;
            var gender = Faker().PickRandom<Gender>();
            var name = Faker().Name.FullName();
            var verifiedIdentifiers = new[] {new Identifier(IdentifierType.MOBILE, Faker().Phone.PhoneNumber())};
            var unverifiedIdentifiers = new[] {new Identifier(IdentifierType.MR, patientReferenceNumber)};
            var patientRequest = new PatientEnquiry(consentManagerUserId,
                verifiedIdentifiers,
                unverifiedIdentifiers,
                name,
                gender,
                yearOfBirth);
            var discoveryRequest = new DiscoveryRequest(patientRequest, Faker().Random.String(), RandomString(), DateTime.Now);
            linkPatientRepository.Setup(e => e.GetLinkedCareContexts(consentManagerUserId))
                .ReturnsAsync(new Tuple<IEnumerable<LinkedAccounts>, Exception>(new List<LinkedAccounts>(), null));

            matchingRepository
                .Setup(repo => repo.Where(discoveryRequest))
                .Returns(Task.FromResult(new List<Patient>
                {
                    new Patient
                    {
                        Identifier = patientReferenceNumber,
                        YearOfBirth = yearOfBirth,
                        Gender = gender,
                        Name = name
                    },
                    new Patient
                    {
                        Identifier = patientReferenceNumber,
                        YearOfBirth = yearOfBirth,
                        Gender = gender,
                        Name = name
                    }
                }.AsQueryable()));

            var (discoveryResponse, error) = await patientDiscovery.PatientFor(discoveryRequest);

            discoveryResponse.Should().BeNull();
            error.Should().BeEquivalentTo(expectedError);
        }

        [Fact]
        private async void ShouldGetNoPatientFoundErrorWhenVerifiedIdentifierDoesNotMatch()
        {
            var patientDiscovery = new PatientDiscovery(
                matchingRepository.Object,
                discoveryRequestRepository.Object,
                linkPatientRepository.Object,
                patientRepository.Object);
            var consentManagerUserId = Faker().Random.String();
            var expectedError = new ErrorRepresentation(new Error(ErrorCode.NoPatientFound, "No patient found"));
            var verifiedIdentifiers = new List<Identifier>
            {
                new Identifier(IdentifierType.MR, Faker().Phone.PhoneNumber())
            };
            var patientRequest = new PatientEnquiry(consentManagerUserId,
                verifiedIdentifiers,
                new List<Identifier>(),
                null,
                Gender.M,
                2019);
            var discoveryRequest = new DiscoveryRequest(patientRequest, Faker().Random.String(), RandomString(), DateTime.Now);
            linkPatientRepository.Setup(e => e.GetLinkedCareContexts(consentManagerUserId))
                .ReturnsAsync(new Tuple<IEnumerable<LinkedAccounts>, Exception>(new List<LinkedAccounts>(), null));

            var (discoveryResponse, error) = await patientDiscovery.PatientFor(discoveryRequest);

            discoveryResponse.Should().BeNull();
            error.Should().BeEquivalentTo(expectedError);
        }

        [Theory]
        [ClassData(typeof(EmptyIdentifierTestData))]
        private async void ReturnNoPatientFoundErrorWhenVerifiedIdentifierIs(IEnumerable<Identifier> identifiers)
        {
            var patientDiscovery = new PatientDiscovery(
                matchingRepository.Object,
                discoveryRequestRepository.Object,
                linkPatientRepository.Object,
                patientRepository.Object);
            var consentManagerUserId = Faker().Random.String();
            var expectedError = new ErrorRepresentation(new Error(ErrorCode.NoPatientFound, "No patient found"));
            var patientRequest = new PatientEnquiry(consentManagerUserId,
                identifiers,
                new List<Identifier>(),
                null,
                Gender.M,
                2019);
            var discoveryRequest = new DiscoveryRequest(patientRequest, Faker().Random.String(), RandomString(), DateTime.Now);
            linkPatientRepository.Setup(e => e.GetLinkedCareContexts(consentManagerUserId))
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
                new ErrorRepresentation(new Error(ErrorCode.DuplicateDiscoveryRequest, "Discovery Request already exists"));
            var transactionId = RandomString();
            var discoveryRequest = new DiscoveryRequest(null, RandomString(),transactionId, DateTime.Now);
            discoveryRequestRepository.Setup(repository => repository.RequestExistsFor(transactionId))
                .ReturnsAsync(true);

            var (discoveryResponse, error) = await patientDiscovery.PatientFor(discoveryRequest);

            discoveryResponse.Should().BeNull();
            error.Should().BeEquivalentTo(expectedError);
        }
    }

    internal class EmptyIdentifierTestData : TheoryData<IEnumerable<Identifier>>
    {
        public EmptyIdentifierTestData()
        {
            Add(null);
            Add(new Identifier[] { });
        }
    }
}