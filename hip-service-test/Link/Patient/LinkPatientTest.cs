using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using hip_service.Link.Patient;
using hip_service.Link.Patient.Models;
using hip_service.OTP;
using hip_service_test.Link.Builder;
using HipLibrary.Patient.Model;
using HipLibrary.Patient.Model.Request;
using HipLibrary.Patient.Model.Response;
using Moq;
using Optional;
using Xunit;
using LinkPatient = hip_service.Link.Patient.LinkPatient;
using LinkLib = HipLibrary.Patient.Model.Request.Link;
using CareContextSer = hip_service.Discovery.Patient.Model.CareContext;
using PatientLinkRequest = HipLibrary.Patient.Model.Request.PatientLinkRequest;
using PatientSer = hip_service.Discovery.Patient.Model.Patient;

namespace hip_service_test.Link.Patient
{
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
            DateOfBirth = DateTime.ParseExact("2019-12-06","yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture),
            Email = TestBuilder.Faker().Random.Words(),
        };
        
        private readonly LinkPatient linkPatient;
        private readonly Mock<ILinkPatientRepository> linkRepository = new Mock<ILinkPatientRepository>();
        private readonly Mock<IPatientRepository> patientRepository = new Mock<IPatientRepository>();
        private readonly Mock<IPatientVerification> patientVerification = new Mock<IPatientVerification>();
        private readonly Mock<IReferenceNumberGenerator> guidGenerator = new Mock<IReferenceNumberGenerator>();

        public LinkPatientTest()
        {
            linkPatient = new LinkPatient(linkRepository.Object, patientRepository.Object, 
                patientVerification.Object, guidGenerator.Object);
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
                , new Communication(CommunicationMode.MOBILE, testPatient.PhoneNumber)))).ReturnsAsync((OtpMessage)null);
            linkRepository.Setup(expression: x => x.SaveRequestWith(linkReferenceNumber,
            patientReferenceRequest.Patient.ConsentManagerId, patientReferenceRequest.Patient.ConsentManagerUserId,
            patientReferenceRequest.Patient.ReferenceNumber, new []{programRefNo}))
                .ReturnsAsync(new Tuple<LinkRequest, Exception>(null,null));
            patientRepository.Setup(x => x.PatientWith(testPatient.Identifier))
                .Returns(Option.Some(testPatient));
            var careContext = new CareContextSer {Description = TestBuilder.Faker().Random.Words()
                , ReferenceNumber = programRefNo};
            patientRepository.Setup(x => x.ProgramInfoWith(testPatient.Identifier, programRefNo))
                .Returns(Option.Some(careContext));
            
            var (response, _) = await linkPatient.LinkPatients(patientReferenceRequest);
            
            patientVerification.Verify();
            linkRepository.Verify();
            guidGenerator.Verify();
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
                .Returns(Option.Some<hip_service.Discovery.Patient.Model.Patient>(testPatient));
            patientRepository.Setup(e => e.ProgramInfoWith(testPatient.Identifier
                , careContexts.First().ReferenceNumber)).Returns(null);
            var expectedError = new ErrorResponse(new Error(ErrorCode.CareContextNotFound,
                "Care context not found for given patient"));
            
            var (_, error) = await linkPatient.LinkPatients(patientReferenceRequest);
            
            patientRepository.Verify();
            error.Should().BeEquivalentTo(expectedError);
        }

        [Fact]
        private async void ReturnOtpInvalidOnWrongOtp()
        {
            var sessionId = TestBuilder.Faker().Random.Hash();
            var otpToken = TestBuilder.Faker().Random.Number().ToString();
            var testOtpMessage = new OtpMessage("1001","Invalid Otp");
            var patientLinkRequest = new PatientLinkRequest(otpToken
                ,sessionId);
            var expectedErrorResponse = new ErrorResponse(new Error(ErrorCode.OtpInValid, testOtpMessage.Message));
            patientVerification.Setup(e => e.Verify(sessionId, otpToken))
                .ReturnsAsync(testOtpMessage);

            var (_, error) = await linkPatient.VerifyAndLinkCareContext(patientLinkRequest);

            patientVerification.Verify();
            error.Should().BeEquivalentTo(expectedErrorResponse);
        }
        
        [Fact]
        private async void ErrorOnInvalidLinkReferenceNumber()
        {
            var sessionId = TestBuilder.Faker().Random.Hash();
            var otpToken = TestBuilder.Faker().Random.Number().ToString();
            var patientLinkRequest = new PatientLinkRequest(otpToken
                ,sessionId);
            var expectedErrorResponse = new ErrorResponse(new Error(ErrorCode.NoLinkRequestFound, "No request found"));
            patientVerification.Setup(e => e.Verify(sessionId, otpToken))
                .ReturnsAsync((OtpMessage)null);
            linkRepository.Setup(e => e.GetPatientFor(sessionId))
                .ReturnsAsync(new Tuple<LinkRequest, Exception>(null, new Exception()));

            var (_, error) = await linkPatient.VerifyAndLinkCareContext(patientLinkRequest);

            patientVerification.Verify();
            error.Should().BeEquivalentTo(expectedErrorResponse);
        }

        [Fact]
        private async void SuccessLinkPatientForValidOtp()
        {
            const string programRefNo = "129";
            var sessionId = TestBuilder.Faker().Random.Hash();
            var otpToken = TestBuilder.Faker().Random.Number().ToString();
            var patientLinkRequest = new PatientLinkRequest(otpToken
                ,sessionId);
            ICollection<LinkedCareContext> linkedCareContext = new [] {new LinkedCareContext(programRefNo)}; 
            var testLinkRequest = new LinkRequest(testPatient.Identifier, sessionId,
                TestBuilder.Faker().Random.Hash(), TestBuilder.Faker().Random.Hash()
                ,It.IsAny<string>(),linkedCareContext);
            patientVerification.Setup(e => e.Verify(sessionId, otpToken))
                .ReturnsAsync((OtpMessage)null);
            linkRepository.Setup(e => e.GetPatientFor(sessionId))
                .ReturnsAsync(new Tuple<LinkRequest, Exception>(testLinkRequest, null));
            patientRepository.Setup(x => x.PatientWith(testPatient.Identifier))
                .Returns(Option.Some(testPatient));
            var careContext = new CareContextSer {Description = TestBuilder.Faker().Random.Words()
                , ReferenceNumber = programRefNo};
            patientRepository.Setup(x => x.ProgramInfoWith(testPatient.Identifier, programRefNo))
                .Returns(Option.Some(careContext));
            var expectedLinkResponse = new PatientLinkResponse(new HipLibrary.Patient.Model.Response.LinkPatient(
                testPatient.Identifier
                , testPatient.FirstName + " " + testPatient.LastName
                , new []{new CareContextRepresentation("129","National Cancer program")}));
            
            var (response, _) = await linkPatient.VerifyAndLinkCareContext(patientLinkRequest);
            
            patientVerification.Verify();
            linkRepository.Verify();
            guidGenerator.Verify();
            response.Patient.ReferenceNumber.Should().BeEquivalentTo(expectedLinkResponse.Patient.ReferenceNumber);
            response.Patient.Display.Should().BeEquivalentTo(expectedLinkResponse.Patient.Display);
        }
    }
}