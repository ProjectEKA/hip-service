using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using hip_service.Link.Patient;
using hip_service.Link.Patient.Models;
using hip_service.OTP;
using hip_service_test.Link.Builder;
using HipLibrary.Patient.Model.Request;
using HipLibrary.Patient.Model.Response;
using Moq;
using Optional;
using Xunit;
using LinkPatient = hip_service.Link.Patient.LinkPatient;
using LinkLib = HipLibrary.Patient.Model.Request.Link;
using CareContextSer = hip_service.Discovery.Patient.Model.CareContext;
using PatientSer = hip_service.Discovery.Patient.Model.Patient;

namespace hip_service_test.Link.Patient
{
    public class LinkPatientTest
    {
        private static readonly string patientReferenceNumber = "4";
        private static readonly string phoneNumber = "+91666666666666";
        private readonly PatientSer testPatient = 
            new PatientSer
        {
            PhoneNumber = phoneNumber,
            Identifier = patientReferenceNumber,
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
        private readonly Mock<IGuidWrapper> guidWrapper = new Mock<IGuidWrapper>();

        public LinkPatientTest()
        {
            linkPatient = new LinkPatient(linkRepository.Object, patientRepository.Object, 
                patientVerification.Object, guidWrapper.Object);
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
                TestBuilder.Faker().Random.Hash(), patientReferenceNumber, careContexts);
            var patientReferenceRequest = new PatientLinkReferenceRequest(TestBuilder.Faker().Random.Hash(), patient);
            guidWrapper.Setup(x => x.NewGuid()).Returns(linkReferenceNumber);
            patientVerification.Setup(x => x.SendTokenFor(new Session(linkReferenceNumber
                , new Communication(CommunicationMode.MOBILE, phoneNumber)))).ReturnsAsync((Error)null);
            linkRepository.Setup(expression: x => x.SaveLinkPatientDetails(linkReferenceNumber,
            patientReferenceRequest.Patient.ConsentManagerId, patientReferenceRequest.Patient.ConsentManagerUserId,
            patientReferenceRequest.Patient.ReferenceNumber, new []{programRefNo}))
                .ReturnsAsync(new Tuple<LinkRequest, Exception>(null,null));
            patientRepository.Setup(x => x.GetPatientInfoWithReferenceNumber(patientReferenceNumber))
                .Returns(Option.Some(testPatient));
            var careContext = new CareContextSer {Description = TestBuilder.Faker().Random.Words()
                , ReferenceNumber = programRefNo};
            patientRepository.Setup(x => x.GetProgramInfo(patientReferenceNumber, programRefNo))
                .Returns(Option.Some(careContext));
            
            var (response, _) = await linkPatient.LinkPatients(patientReferenceRequest);
            
            patientVerification.Verify();
            linkRepository.Verify();
            guidWrapper.Verify();
            response.Link.ReferenceNumber.Should().Be(linkReferenceNumber);
            response.Link.AuthenticationType.Should().Be(authType);
            response.Link.Meta.CommunicationHint.Should().Be(phoneNumber);
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
            patientRepository.Setup(e => e.GetPatientInfoWithReferenceNumber(patientReferenceNumber))
                .Returns(Option.Some<hip_service.Discovery.Patient.Model.Patient>(testPatient));
            patientRepository.Setup(e => e.GetProgramInfo(patientReferenceNumber
                , careContexts.First().ReferenceNumber)).Returns(null);
            var expectedError = new ErrorResponse(new Error(ErrorCode.CareContextNotFound,
                "Care context not found for given patient"));
            
            var (_, error) = await linkPatient.LinkPatients(patientReferenceRequest);
            
            patientRepository.Verify();
            error.Should().BeEquivalentTo(expectedError);
        }
    }
}