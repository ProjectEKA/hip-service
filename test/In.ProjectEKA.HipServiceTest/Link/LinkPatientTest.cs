namespace In.ProjectEKA.HipServiceTest.Link
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Moq;
    using Optional;
    using Xunit;
    using System.Linq;
    using Builder;
    using HipLibrary.Patient;
    using HipLibrary.Patient.Model;
    using HipService.Discovery;
    using HipService.Link;
    using HipService.Link.Model;
    using Microsoft.Extensions.Options;
    using LinkPatient = HipService.Link.LinkPatient;
    using PatientSer = HipLibrary.Patient.Model.Patient;

    public class LinkPatientTest
    {
        private readonly PatientSer testPatient =
            new PatientSer
            {
                PhoneNumber = "+91666666666666",
                Identifier = "4",
                Gender = TestBuilders.Faker().PickRandom<Gender>(),
                Name = TestBuilders.Faker().Random.Word(),
                CareContexts = new List<CareContextRepresentation>
                {
                    new CareContextRepresentation("129", "National Cancer program")
                }
            };

        private readonly LinkPatient linkPatient;
        private readonly Mock<ILinkPatientRepository> linkRepository = new Mock<ILinkPatientRepository>();

        private readonly Mock<IDiscoveryRequestRepository> discoveryRequestRepository =
            new Mock<IDiscoveryRequestRepository>();

        private readonly Mock<IPatientRepository> patientRepository = new Mock<IPatientRepository>();
        private readonly Mock<IPatientVerification> patientVerification = new Mock<IPatientVerification>();
        private readonly Mock<ReferenceNumberGenerator> guidGenerator = new Mock<ReferenceNumberGenerator>();

        public LinkPatientTest()
        {
            var otpService = new OtpServiceConfiguration {BaseUrl = "http://localhost:5000", OffsetInMinutes = 5};
            var otpServiceConfigurations = Options.Create(otpService);
            linkPatient = new LinkPatient(linkRepository.Object,
                patientRepository.Object,
                patientVerification.Object,
                guidGenerator.Object,
                discoveryRequestRepository.Object,
                otpServiceConfigurations);
        }

        [Fact]
        private async void ShouldReturnLinkReferenceResponse()
        {
            const string linkReferenceNumber = "linkreference";
            const string authType = "MEDIATED";
            const string programRefNo = "129";
            const string medium = "MOBILE";

            IEnumerable<CareContextEnquiry> careContexts = new[] {new CareContextEnquiry(programRefNo)};
            var patient = new LinkEnquiry(TestBuilders.Faker().Random.Hash(),
                TestBuilders.Faker().Random.Hash(), testPatient.Identifier, careContexts);
            var patientReferenceRequest = new PatientLinkEnquiry(TestBuilders.Faker().Random.Hash(), patient);
            guidGenerator.Setup(x => x.NewGuid()).Returns(linkReferenceNumber);
            patientVerification.Setup(x => x.SendTokenFor(new Session(linkReferenceNumber
                    , new Communication(CommunicationMode.MOBILE, testPatient.PhoneNumber))))
                .ReturnsAsync((OtpMessage) null);

            linkRepository.Setup(x => x.SaveRequestWith(linkReferenceNumber,
                    patientReferenceRequest.Patient.ConsentManagerId,
                    patientReferenceRequest.Patient.ConsentManagerUserId,
                    patientReferenceRequest.Patient.ReferenceNumber, new[] {programRefNo}))
                .ReturnsAsync(new Tuple<LinkEnquires, Exception>(null, null));

            patientRepository.Setup(x => x.PatientWith(testPatient.Identifier))
                .Returns(Option.Some(testPatient));

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
            IEnumerable<CareContextEnquiry> careContexts = new[] {new CareContextEnquiry("129")};
            var patient = new LinkEnquiry(TestBuilders.Faker().Random.Hash(),
                TestBuilders.Faker().Random.Hash(), "1234", careContexts);
            var patientReferenceRequest = new PatientLinkEnquiry(TestBuilders.Faker().Random.Hash(), patient);

            var expectedError =
                new ErrorRepresentation(new Error(ErrorCode.NoPatientFound, ErrorMessage.NoPatientFound));
            var (_, error) = await linkPatient.LinkPatients(patientReferenceRequest);

            error.Should().BeEquivalentTo(expectedError);
        }

        [Fact]
        private async void ShouldReturnCareContextNotFoundError()
        {
            IEnumerable<CareContextEnquiry> careContexts = new[] {new CareContextEnquiry("1234")};
            var patient = new LinkEnquiry(TestBuilders.Faker().Random.Hash(),
                TestBuilders.Faker().Random.Hash(), "4", careContexts);
            var patientReferenceRequest = new PatientLinkEnquiry(TestBuilders.Faker().Random.Hash(), patient);
            patientRepository.Setup(e => e.PatientWith(testPatient.Identifier))
                .Returns(Option.Some(testPatient));
            var expectedError = new ErrorRepresentation(
                new Error(ErrorCode.CareContextNotFound, ErrorMessage.CareContextNotFound));

            var (_, error) = await linkPatient.LinkPatients(patientReferenceRequest);

            patientRepository.Verify();
            discoveryRequestRepository.Invocations.Count.Should().Be(0);
            error.Should().BeEquivalentTo(expectedError);
        }

        [Fact]
        private async void ReturnOtpInvalidOnWrongOtp()
        {
            var sessionId = TestBuilders.Faker().Random.Hash();
            var otpToken = TestBuilders.Faker().Random.Number().ToString();
            var testOtpMessage = new OtpMessage(ResponseType.OtpInvalid, "Invalid Otp");
            var patientLinkRequest = new LinkConfirmationRequest(otpToken, sessionId);
            var expectedErrorResponse =
                new ErrorRepresentation(new Error(ErrorCode.OtpInValid, testOtpMessage.Message));
            patientVerification.Setup(e => e.Verify(sessionId, otpToken))
                .ReturnsAsync(testOtpMessage);

            var (_, error) = await linkPatient.VerifyAndLinkCareContext(patientLinkRequest);

            patientVerification.Verify();
            error.Should().BeEquivalentTo(expectedErrorResponse);
        }

        [Fact]
        private async void ReturnOtpExpired()
        {
            var sessionId = TestBuilders.Faker().Random.Hash();
            var otpToken = TestBuilders.Faker().Random.Number().ToString();
            var testOtpMessage = new OtpMessage(ResponseType.OtpExpired, "Otp Expired");
            var patientLinkRequest = new LinkConfirmationRequest(otpToken, sessionId);
            var expectedErrorResponse =
                new ErrorRepresentation(new Error(ErrorCode.OtpExpired, testOtpMessage.Message));
            patientVerification.Setup(e => e.Verify(sessionId, otpToken))
                .ReturnsAsync(testOtpMessage);

            var (_, error) = await linkPatient.VerifyAndLinkCareContext(patientLinkRequest);

            patientVerification.Verify();
            error.Should().BeEquivalentTo(expectedErrorResponse);
        }

        [Fact]
        private async void ErrorOnInvalidLinkReferenceNumber()
        {
            var sessionId = TestBuilders.Faker().Random.Hash();
            var otpToken = TestBuilders.Faker().Random.Number().ToString();
            var patientLinkRequest = new LinkConfirmationRequest(otpToken, sessionId);
            var expectedErrorResponse =
                new ErrorRepresentation(new Error(ErrorCode.NoLinkRequestFound, "No request found"));
            patientVerification.Setup(e => e.Verify(sessionId, otpToken))
                .ReturnsAsync((OtpMessage) null);
            linkRepository.Setup(e => e.GetPatientFor(sessionId))
                .ReturnsAsync(new Tuple<LinkEnquires, Exception>(null, new Exception()));


            var (_, error) = await linkPatient.VerifyAndLinkCareContext(patientLinkRequest);

            patientVerification.Verify();
            error.Should().BeEquivalentTo(expectedErrorResponse);
        }

        [Fact]
        private async void SuccessLinkPatientForValidOtp()
        {
            const string programRefNo = "129";
            var sessionId = TestBuilders.Faker().Random.Hash();
            var otpToken = TestBuilders.Faker().Random.Number().ToString();
            var patientLinkRequest = new LinkConfirmationRequest(otpToken, sessionId);
            ICollection<CareContext> linkedCareContext = new[] {new CareContext(programRefNo)};
            var testLinkRequest = new LinkEnquires(testPatient.Identifier, sessionId,
                TestBuilders.Faker().Random.Hash(), TestBuilders.Faker().Random.Hash()
                , It.IsAny<string>(), linkedCareContext);
            var testLinkedAccounts = new LinkedAccounts(testLinkRequest.PatientReferenceNumber,
                testLinkRequest.LinkReferenceNumber,
                testLinkRequest.ConsentManagerUserId, It.IsAny<string>(), new[] {programRefNo}.ToList());
            patientVerification.Setup(e => e.Verify(sessionId, otpToken))
                .ReturnsAsync((OtpMessage) null);
            linkRepository.Setup(e => e.GetPatientFor(sessionId))
                .ReturnsAsync(new Tuple<LinkEnquires, Exception>(testLinkRequest, null));
            patientRepository.Setup(x => x.PatientWith(testPatient.Identifier))
                .Returns(Option.Some(testPatient));
            linkRepository.Setup(x => x.Save(testLinkRequest.ConsentManagerUserId,
                    testLinkRequest.PatientReferenceNumber,
                    testLinkRequest.LinkReferenceNumber,
                    new[] {programRefNo}))
                .ReturnsAsync(Option.Some(testLinkedAccounts));
            var expectedLinkResponse = new PatientLinkConfirmationRepresentation(
                new LinkConfirmationRepresentation(
                    testPatient.Identifier,
                    $"{testPatient.Name}",
                    new[] {new CareContextRepresentation("129", "National Cancer program")}));

            var (response, _) = await linkPatient.VerifyAndLinkCareContext(patientLinkRequest);

            patientVerification.Verify();
            linkRepository.Verify();
            guidGenerator.Verify();
            response.Patient.ReferenceNumber.Should().BeEquivalentTo(expectedLinkResponse.Patient.ReferenceNumber);
            response.Patient.Display.Should().BeEquivalentTo(expectedLinkResponse.Patient.Display);
        }
    }
}