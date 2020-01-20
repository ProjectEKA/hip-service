namespace In.ProjectEKA.HipServiceTest.Link.Patient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Builder;
    using DefaultHip.Discovery;
    using FluentAssertions;
    using HipLibrary.Patient.Model.Request;
    using HipLibrary.Patient.Model.Response;
    using HipService.Link.Patient;
    using HipService.Link.Patient.Model;
    using Moq;
    using Optional;
    using Xunit;
    using LinkPatient = HipService.Link.Patient.LinkPatient;
    using LinkLib = HipLibrary.Patient.Model.Request.Link;
    using CareContextSer = In.ProjectEKA.DefaultHip.Discovery.Model.CareContext;
    using PatientSer = In.ProjectEKA.DefaultHip.Discovery.Model.Patient;

    public class LinkPatientTest
    {
        private readonly PatientSer testPatient =
            new PatientSer
            {
                PhoneNumber = "+91666666666666",
                Identifier = "4",
                FirstName = TestBuilder.Faker().Random.Word(),
                LastName = TestBuilder.Faker().Random.Word(),
                Gender = TestBuilder.Faker().Random.Word(),
                DateOfBirth = DateTime.ParseExact("2019-12-06", "yyyy-MM-dd",
                    System.Globalization.CultureInfo.InvariantCulture),
                Email = TestBuilder.Faker().Random.Words(),
            };

        private readonly LinkPatient linkPatient;
        private readonly Mock<ILinkPatientRepository> linkRepository = new Mock<ILinkPatientRepository>();

        private readonly Mock<IDiscoveryRequestRepository> discoveryRequestRepository =
            new Mock<IDiscoveryRequestRepository>();

        private readonly Mock<IPatientRepository> patientRepository = new Mock<IPatientRepository>();
        private readonly Mock<IPatientVerification> patientVerification = new Mock<IPatientVerification>();
        private readonly Mock<IReferenceNumberGenerator> guidGenerator = new Mock<IReferenceNumberGenerator>();

        public LinkPatientTest()
        {
            linkPatient = new LinkPatient(linkRepository.Object, patientRepository.Object,
                patientVerification.Object, guidGenerator.Object, discoveryRequestRepository.Object);
        }

        [Fact]
        private async void ShouldReturnLinkReferenceResponse()
        {
            const string linkReferenceNumber = "linkreference";
            const string authType = "MEDIATED";
            const string programRefNo = "129";
            const string medium = "MOBILE";

            IEnumerable<CareContext> careContexts = new[] {new CareContext(programRefNo)};
            var patient = new LinkLib(TestBuilder.Faker().Random.Hash(),
                TestBuilder.Faker().Random.Hash(), testPatient.Identifier, careContexts);
            var patientReferenceRequest = new PatientLinkReferenceRequest(TestBuilder.Faker().Random.Hash(), patient);
            guidGenerator.Setup(x => x.NewGuid()).Returns(linkReferenceNumber);
            patientVerification.Setup(x => x.SendTokenFor(new Session(linkReferenceNumber
                , new Communication(CommunicationMode.MOBILE, testPatient.PhoneNumber)))).ReturnsAsync((Error) null);
            linkRepository.Setup(expression: x => x.SaveRequestWith(linkReferenceNumber,
                    patientReferenceRequest.Patient.ConsentManagerId,
                    patientReferenceRequest.Patient.ConsentManagerUserId,
                    patientReferenceRequest.Patient.ReferenceNumber, new[] {programRefNo}))
                .ReturnsAsync(new Tuple<LinkRequest, Exception>(null, null));
            patientRepository.Setup(x => x.PatientWith(testPatient.Identifier))
                .Returns(Option.Some(testPatient));
            var careContext = new CareContextSer
                {Description = TestBuilder.Faker().Random.Words(), ReferenceNumber = programRefNo};
            patientRepository.Setup(x => x.ProgramInfoWith(testPatient.Identifier, programRefNo))
                .Returns(Option.Some(careContext));

            var (response, _) = await linkPatient.LinkPatients(patientReferenceRequest);

            patientVerification.Verify();
            linkRepository.Verify();
            guidGenerator.Verify();
            discoveryRequestRepository.Verify(x => x.Delete(patientReferenceRequest.TransactionId,
                patientReferenceRequest.Patient.ConsentManagerUserId));
            response.Link.ReferenceNumber.Should().Be(linkReferenceNumber);
            response.Link.AuthenticationType.Should().Be(authType);
            response.Link.Meta.CommunicationHint.Should().Be(testPatient.PhoneNumber);
            response.Link.Meta.CommunicationMedium.Should().Be(medium);
            response.Link.Should().NotBeNull();
        }

        [Fact]
        private async void ShouldReturnPatientNotFoundError()
        {
            IEnumerable<CareContext> careContexts = new[] {new CareContext("129")};
            var patient = new LinkLib(TestBuilder.Faker().Random.Hash(),
                TestBuilder.Faker().Random.Hash(), "1234", careContexts);
            var patientReferenceRequest = new PatientLinkReferenceRequest(TestBuilder.Faker().Random.Hash(), patient);

            var expectedError = new ErrorResponse(new Error(ErrorCode.NoPatientFound, "No patient Found"));
            var (_, error) = await linkPatient.LinkPatients(patientReferenceRequest);

            error.Should().BeEquivalentTo(expectedError);
        }

        [Fact]
        private async void ShouldReturnCareContextNotFoundError()
        {
            IEnumerable<CareContext> careContexts = new[] {new CareContext("1234")};
            var patient = new LinkLib(TestBuilder.Faker().Random.Hash(),
                TestBuilder.Faker().Random.Hash(), "4", careContexts);
            var patientReferenceRequest = new PatientLinkReferenceRequest(TestBuilder.Faker().Random.Hash(), patient);
            patientRepository.Setup(e => e.PatientWith(testPatient.Identifier))
                .Returns(Option.Some(testPatient));
            patientRepository.Setup(e => e.ProgramInfoWith(testPatient.Identifier
                , careContexts.First().ReferenceNumber)).Returns(null);
            var expectedError = new ErrorResponse(new Error(ErrorCode.CareContextNotFound,
                "Care context not found for given patient"));

            var (_, error) = await linkPatient.LinkPatients(patientReferenceRequest);

            patientRepository.Verify();
            discoveryRequestRepository.Invocations.Count.Should().Be(0);
            error.Should().BeEquivalentTo(expectedError);
        }
    }
}