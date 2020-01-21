using System;
using FluentAssertions;
using HipLibrary.Patient;
using HipLibrary.Patient.Model;
using HipLibrary.Patient.Model.Request;
using HipLibrary.Patient.Model.Response;
using In.ProjectEKA.DefaultHip.Link;
using In.ProjectEKA.HipService.Link;
using In.ProjectEKA.HipServiceTest.Link.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace In.ProjectEKA.HipServiceTest.Link
{
    using PatientLinkRefRequest = HipLibrary.Patient.Model.Request.PatientLinkReferenceRequest;
    using LinkLib = HipLibrary.Patient.Model.Request.Link;
    using LinkPatient = HipLibrary.Patient.Model.Response.LinkPatient;
    using LinkReferenceLib = HipLibrary.Patient.Model.Response.LinkReference;
    using PatientLinkRequest = HipLibrary.Patient.Model.Request.PatientLinkRequest;
    
    [Collection("Link Patient Controller Tests")]
    public class LinkPatientControllerTest
    {
        private readonly Mock<ILink> link;
        private readonly LinkPatientController linkPatientController;

        public LinkPatientControllerTest()
        {
            link = new Mock<ILink>();
            linkPatientController = new LinkPatientController(link.Object);
        }

        [Fact]
        private async void ReturnLinkReferenceNumber()
        {
            var faker = TestBuilder.Faker();
            const string programRefNo = "129";
            const string patientReference = "4";
            var consentManagerId = faker.Random.Hash();
            var consentManagerUserId = faker.Random.Hash();
            var transactionId = faker.Random.Hash();
            var linkRequest = new PatientLinkRefRequest(transactionId
                , new LinkLib(consentManagerId,consentManagerUserId
                    , referenceNumber:patientReference, new [] {new CareContext(programRefNo)}));
            var linkReference = new LinkReferenceLib(faker.Random.Hash(),"MEDIATED"
                , new LinkReferenceMeta("MOBILE","+91666666666666"
                    ,It.IsAny<string>()));
            var expectedResponse = new PatientLinkReferenceResponse(linkReference);
            link.Setup(e => e.LinkPatients(
                    It.Is<PatientLinkRefRequest>(p =>
                            p.TransactionId == linkRequest.TransactionId &&
                            p.Patient.ReferenceNumber == linkRequest.Patient.ReferenceNumber &&
                            p.Patient.ConsentManagerId == consentManagerId &&
                            p.Patient.ConsentManagerUserId == linkRequest.Patient.ConsentManagerUserId &&
                            Equals(p.Patient.CareContexts, linkRequest.Patient.CareContexts)
                    )   
                    ))
                .ReturnsAsync(new Tuple<PatientLinkReferenceResponse, ErrorResponse>(expectedResponse, null));

            var response =
                await linkPatientController.LinkPatientCareContexts(consentManagerId,
                    linkRequest);
            
            link.Verify();
            response.Should()
                .NotBeNull()
                .And
                .Subject.As<OkObjectResult>()
                .Value
                .Should()
                .BeEquivalentTo(expectedResponse);
        }


        [Fact]
        private async void ReturnLinkedPatientAndCareContext()
        {
            var linkReferenceNumber = TestBuilder.Faker().Random.Hash();
            var linkPatientWithToken = new HipService.Link.PatientLinkRequest("1234");
            var careContext = new[] {new CareContextRepresentation("129", TestBuilder.Faker().Random.Word())};
            var expectedResponse = new PatientLinkResponse(new LinkPatient("4",TestBuilder.Faker().Random.Word()
                ,careContext));
            link.Setup(e => e.VerifyAndLinkCareContext(It.Is<PatientLinkRequest>(p =>
                    p.Token == "1234" &&
                    p.LinkReferenceNumber == linkReferenceNumber)))
                .ReturnsAsync(new Tuple<PatientLinkResponse, ErrorResponse>(expectedResponse, null));

            var response =
                await linkPatientController.LinkPatient(linkReferenceNumber, linkPatientWithToken);
            
            link.Verify();
            response.Should()
                .NotBeNull()
                .And
                .Subject.As<OkObjectResult>()
                .Value
                .Should()
                .BeEquivalentTo(expectedResponse);
        }

        [Fact]
        private async void CheckOtpGenerationError()
        {
            var faker = TestBuilder.Faker();
            var consentManagerId = faker.Random.Hash();
            var linkRequest = new PatientLinkRefRequest(faker.Random.Hash()
                , new LinkLib(faker.Random.Hash(),faker.Random.Hash(),faker.Random.Hash()
                    , new [] {new CareContext(faker.Random.Hash())}));
            var expectedError = new ErrorResponse(new Error(ErrorCode.OtpGenerationFailed, "Otp Generation Failed"));
            link.Setup(e => e.LinkPatients(
                    It.Is<PatientLinkRefRequest>(p =>
                        p.TransactionId == linkRequest.TransactionId &&
                        p.Patient.ReferenceNumber == linkRequest.Patient.ReferenceNumber &&
                        p.Patient.ConsentManagerId == consentManagerId &&
                        p.Patient.ConsentManagerUserId == linkRequest.Patient.ConsentManagerUserId &&
                        Equals(p.Patient.CareContexts, linkRequest.Patient.CareContexts)
                    )   
                ))
                .ReturnsAsync(new Tuple<PatientLinkReferenceResponse, ErrorResponse>(null, expectedError));

            var response = await linkPatientController.LinkPatientCareContexts(consentManagerId, linkRequest);
            
            link.Verify();
            response.Should().NotBeNull()
                .And
                .BeOfType<BadRequestObjectResult>()
                .Subject.StatusCode.Should()
                .Be(StatusCodes.Status400BadRequest);
        }
        
        [Fact]
        private async void CheckDataStorageError()
        {
            var faker = TestBuilder.Faker();
            var consentManagerId = faker.Random.Hash();
            var linkRequest = new PatientLinkRefRequest(faker.Random.Hash()
                , new LinkLib(faker.Random.Hash(),faker.Random.Hash(),faker.Random.Hash()
                    , new [] {new CareContext(faker.Random.Hash())}));
            var expectedError = new ErrorResponse(new Error(ErrorCode.ServerInternalError, ErrorMessage.DatabaseStorageError));
            link.Setup(e => e.LinkPatients(
                    It.Is<PatientLinkRefRequest>(p =>
                        p.TransactionId == linkRequest.TransactionId &&
                        p.Patient.ReferenceNumber == linkRequest.Patient.ReferenceNumber &&
                        p.Patient.ConsentManagerId == consentManagerId &&
                        p.Patient.ConsentManagerUserId == linkRequest.Patient.ConsentManagerUserId &&
                        Equals(p.Patient.CareContexts, linkRequest.Patient.CareContexts)
                    )   
                ))
                .ReturnsAsync(new Tuple<PatientLinkReferenceResponse, ErrorResponse>(null, expectedError));

            var response = await linkPatientController.LinkPatientCareContexts(consentManagerId, linkRequest);
            
            link.Verify();
            response.Should().NotBeNull()
                .And
                .BeOfType<BadRequestObjectResult>()
                .Subject.StatusCode.Should()
                .Be(StatusCodes.Status400BadRequest);
        }
        
        [Fact]
        private async void CheckCareContextError()
        {
            var faker = TestBuilder.Faker();
            var consentManagerId = faker.Random.Hash();
            var linkRequest = new PatientLinkRefRequest(faker.Random.Hash()
                , new LinkLib(faker.Random.Hash(),faker.Random.Hash(),faker.Random.Hash()
                    , new [] {new CareContext(faker.Random.Hash())}));
            var expectedError = new ErrorResponse(new Error(ErrorCode.CareContextNotFound, "Care context not found for given patient"));
            link.Setup(e => e.LinkPatients(
                    It.Is<PatientLinkRefRequest>(p =>
                        p.TransactionId == linkRequest.TransactionId &&
                        p.Patient.ReferenceNumber == linkRequest.Patient.ReferenceNumber &&
                        p.Patient.ConsentManagerId == consentManagerId &&
                        p.Patient.ConsentManagerUserId == linkRequest.Patient.ConsentManagerUserId &&
                        Equals(p.Patient.CareContexts, linkRequest.Patient.CareContexts)
                    )   
                ))
                .ReturnsAsync(new Tuple<PatientLinkReferenceResponse, ErrorResponse>(null, expectedError));

            var response = await linkPatientController.LinkPatientCareContexts(consentManagerId, linkRequest);
            
            link.Verify();
            response.Should().NotBeNull()
                .And
                .BeOfType<NotFoundObjectResult>()
                .Subject.StatusCode.Should()
                .Be(StatusCodes.Status404NotFound);
        }
        
        [Fact]
        private async void CheckSearchPatientError()
        {
            var faker = TestBuilder.Faker();
            var consentManagerId = faker.Random.Hash();
            var linkRequest = new PatientLinkRefRequest(faker.Random.Hash()
                , new LinkLib(faker.Random.Hash(),faker.Random.Hash(),faker.Random.Hash()
                    , new [] {new CareContext(faker.Random.Hash())}));
            var expectedError = new ErrorResponse(new Error(ErrorCode.CareContextNotFound, "No patient Found"));
            link.Setup(e => e.LinkPatients(
                    It.Is<PatientLinkRefRequest>(p =>
                        p.TransactionId == linkRequest.TransactionId &&
                        p.Patient.ReferenceNumber == linkRequest.Patient.ReferenceNumber &&
                        p.Patient.ConsentManagerId == consentManagerId &&
                        p.Patient.ConsentManagerUserId == linkRequest.Patient.ConsentManagerUserId &&
                        Equals(p.Patient.CareContexts, linkRequest.Patient.CareContexts)
                    )   
                ))
                .ReturnsAsync(new Tuple<PatientLinkReferenceResponse, ErrorResponse>(null, expectedError));

            var response = await linkPatientController.LinkPatientCareContexts(consentManagerId, linkRequest);
            
            link.Verify();
            response.Should().NotBeNull()
                .And
                .BeOfType<NotFoundObjectResult>()
                .Subject.StatusCode.Should()
                .Be(StatusCodes.Status404NotFound);
        }
        
        [Fact]
        private async void CheckInvalidOtpError()
        {
            var faker = TestBuilder.Faker();
            var consentManagerId = faker.Random.Hash();
            var linkRequest = new PatientLinkRefRequest(faker.Random.Hash()
                , new LinkLib(faker.Random.Hash(),faker.Random.Hash(),faker.Random.Hash()
                    , new [] {new CareContext(faker.Random.Hash())}));
            var expectedError = new ErrorResponse(new Error(ErrorCode.OtpInValid, "Otp Invalid"));
            link.Setup(e => e.LinkPatients(
                    It.Is<PatientLinkRefRequest>(p =>
                        p.TransactionId == linkRequest.TransactionId &&
                        p.Patient.ReferenceNumber == linkRequest.Patient.ReferenceNumber &&
                        p.Patient.ConsentManagerId == consentManagerId &&
                        p.Patient.ConsentManagerUserId == linkRequest.Patient.ConsentManagerUserId &&
                        Equals(p.Patient.CareContexts, linkRequest.Patient.CareContexts)
                    )   
                ))
                .ReturnsAsync(new Tuple<PatientLinkReferenceResponse, ErrorResponse>(null, expectedError));

            var response = await linkPatientController.LinkPatientCareContexts(consentManagerId, linkRequest);
            
            link.Verify();
            response.Should().NotBeNull()
                .And
                .BeOfType<NotFoundObjectResult>()
                .Subject.StatusCode.Should()
                .Be(StatusCodes.Status404NotFound);
        }
    }
}