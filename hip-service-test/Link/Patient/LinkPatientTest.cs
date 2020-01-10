using System;
using System.Collections.Generic;
using FluentAssertions;
using hip_service.Link.Patient;
using hip_service.Link.Patient.Models;
using hip_service.OTP;
using hip_service_test.Link.Builder;
using HipLibrary.Patient.Model;
using HipLibrary.Patient.Model.Request;
using HipLibrary.Patient.Model.Response;
using Moq;
using Xunit;
using LinkPatient = hip_service.Link.Patient.LinkPatient;

namespace hip_service_test.Link.Patient
{
    public class LinkPatientTest
    {
        private readonly LinkPatient linkPatient;
        private readonly Mock<ILinkPatientRepository> linkRepository = new Mock<ILinkPatientRepository>();
        private readonly PatientRepository patientRepository = new PatientRepository("patients.json");
        private readonly Mock<IPatientVerification> patientVerification = new Mock<IPatientVerification>();
        private readonly Mock<IGuidWrapper> guidWrapper = new Mock<IGuidWrapper>();

        public LinkPatientTest()
        {
            linkPatient = new LinkPatient(linkRepository.Object, patientRepository, 
                patientVerification.Object, guidWrapper.Object);
        }
        
        [Fact]
        private async void ShouldNotReturnError()
        {
            IEnumerable<CareContext> careContexts = new[] {new CareContext("129")};
            
            var patient = new HipLibrary.Patient.Model.Request.Link(TestBuilder.Faker().Random.Hash(),
                TestBuilder.Faker().Random.Hash(), "4", careContexts); 
           
            var patientReferenceRequest = new HipLibrary.Patient.Model.Request
                .PatientLinkReferenceRequest(TestBuilder.Faker().Random.Hash(), patient);
            
            var linkReferenceNumber = "linkreference";
            guidWrapper.Setup(x => x.NewGuid()).Returns(linkReferenceNumber);

            patientVerification.Setup(x => x.SendTokenFor(new Session(linkReferenceNumber
                , new Communication(CommunicationMode.MOBILE, "666666666666")))).ReturnsAsync((Error)null);

            linkRepository.Setup(expression: x => x.SaveLinkPatientDetails(linkReferenceNumber,
            patientReferenceRequest.Patient.ConsentManagerId, patientReferenceRequest.Patient.ConsentManagerUserId,
            patientReferenceRequest.Patient.ReferenceNumber, new []{"129"}))
                .ReturnsAsync(new Tuple<LinkRequest, Exception>(null,null));
            
            var (response, _) = await linkPatient.LinkPatients(patientReferenceRequest);
            
            patientVerification.Verify();
            linkRepository.Verify();
            guidWrapper.Verify();

            var linkReference = new LinkReference(linkReferenceNumber,"MEDIATED",new LinkReferenceMeta("MOBILE"
                ,"+91666666666666",response.Link.Meta.CommunicationExpiry));

            response.Link.ReferenceNumber.Should().Be(linkReference.ReferenceNumber);
            response.Link.AuthenticationType.Should().Be(linkReference.AuthenticationType);
            response.Link.Meta.CommunicationExpiry.Should().Be(linkReference.Meta.CommunicationExpiry);
            response.Link.Meta.CommunicationHint.Should().Be(linkReference.Meta.CommunicationHint);
            response.Link.Meta.CommunicationMedium.Should().Be(linkReference.Meta.CommunicationMedium);
            response.Link.Should().NotBeNull();
        }
        
        [Fact]
        private async void ShouldReturnPatientNotFoundError()
        {
            IEnumerable<CareContext> careContexts = new[] {new CareContext("129")};
            var patient = new HipLibrary.Patient.Model.Request.Link(TestBuilder.Faker().Random.Hash(),
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
            var patient = new HipLibrary.Patient.Model.Request.Link(TestBuilder.Faker().Random.Hash(),
                TestBuilder.Faker().Random.Hash(), "4", careContexts);
            var patientReferenceRequest = new PatientLinkReferenceRequest(TestBuilder.Faker().Random.Hash(), patient);

            var expectedError = new ErrorResponse(new Error(ErrorCode.CareContextNotFound,
                "Care context not found for given patient"));
            var (_, error) = await linkPatient.LinkPatients(patientReferenceRequest);
            
            error.Should().BeEquivalentTo(expectedError);
        }
    }
}