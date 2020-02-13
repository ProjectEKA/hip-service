using System;
using FluentAssertions;
using In.ProjectEKA.HipService.Link;
using In.ProjectEKA.HipServiceTest.Link.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace In.ProjectEKA.HipServiceTest.Link
{
    using HipLibrary.Patient;
    using HipLibrary.Patient.Model;
    using HipService.Discovery;
    using static TestBuilder;

    [Collection("Link Patient Controller Tests")]
    public class LinkPatientControllerTest
    {
        private readonly Mock<ILink> link;
        private readonly Mock<IDiscoveryRequestRepository> discoveryRequestRepository;
        private readonly LinkPatientController linkPatientController;

        public LinkPatientControllerTest()
        {
            link = new Mock<ILink>();
            discoveryRequestRepository = new Mock<IDiscoveryRequestRepository>();
            linkPatientController = new LinkPatientController(link.Object, discoveryRequestRepository.Object);
        }

        [Fact]
        private async void ReturnLinkReferenceNumber()
        {
            var faker = Faker();
            const string programRefNo = "129";
            const string patientReference = "4";
            var consentManagerId = faker.Random.Hash();
            var consentManagerUserId = faker.Random.Hash();
            var transactionId = faker.Random.Hash();
            var linkRequest = new PatientLinkReferenceRequest(
                transactionId,
                new LinkReference(
                    consentManagerUserId,
                    patientReference,
                    new[] {new CareContextEnquiry(programRefNo)}));
            var linkReference = new LinkEnquiryRepresentation(faker.Random.Hash(), "MEDIATED"
                , new LinkReferenceMeta("MOBILE", "+91666666666666"
                    , It.IsAny<string>()));
            var expectedResponse = new PatientLinkEnquiryRepresentation(linkReference);
            link.Setup(e => e.LinkPatients(
                    It.Is<PatientLinkEnquiry>(p =>
                        p.TransactionId == linkRequest.TransactionId &&
                        p.Patient.ReferenceNumber == linkRequest.Patient.ReferenceNumber &&
                        p.Patient.ConsentManagerId == consentManagerId &&
                        p.Patient.ConsentManagerUserId == linkRequest.Patient.ConsentManagerUserId &&
                        Equals(p.Patient.CareContexts, linkRequest.Patient.CareContexts)
                    )
                ))
                .ReturnsAsync(new Tuple<PatientLinkEnquiryRepresentation, ErrorRepresentation>(expectedResponse, null));
            discoveryRequestRepository.Setup(x => x.RequestExistsFor(linkRequest.TransactionId))
                .ReturnsAsync(true);

            var response =
                await linkPatientController.LinkPatientCareContexts(consentManagerId,
                    linkRequest);

            link.Verify();
            discoveryRequestRepository.Verify();
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
            var linkReferenceNumber = Faker().Random.Hash();
            var linkPatientWithToken = new PatientLinkRequest("1234");
            var careContext = new[] {new CareContextRepresentation("129", Faker().Random.Word())};
            var expectedResponse = new PatientLinkConfirmationRepresentation(new LinkConfirmationRepresentation("4",
                Faker().Random.Word()
                , careContext));
            link.Setup(e => e.VerifyAndLinkCareContext(It.Is<LinkConfirmationRequest>(p =>
                    p.Token == "1234" &&
                    p.LinkReferenceNumber == linkReferenceNumber)))
                .ReturnsAsync(
                    new Tuple<PatientLinkConfirmationRepresentation, ErrorRepresentation>(expectedResponse, null));

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
            var faker = Faker();
            var consentManagerId = faker.Random.Hash();
            var linkRequest = GetFakeLinkRequest();
            var expectedError =
                new ErrorRepresentation(new Error(ErrorCode.OtpGenerationFailed, "Otp Generation Failed"));
            link.Setup(e => e.LinkPatients(
                    It.Is<PatientLinkEnquiry>(p =>
                        p.TransactionId == linkRequest.TransactionId &&
                        p.Patient.ReferenceNumber == linkRequest.Patient.ReferenceNumber &&
                        p.Patient.ConsentManagerId == consentManagerId &&
                        p.Patient.ConsentManagerUserId == linkRequest.Patient.ConsentManagerUserId &&
                        Equals(p.Patient.CareContexts, linkRequest.Patient.CareContexts)
                    )
                ))
                .ReturnsAsync(new Tuple<PatientLinkEnquiryRepresentation, ErrorRepresentation>(null, expectedError));
            discoveryRequestRepository.Setup(x => x.RequestExistsFor(linkRequest.TransactionId))
                .ReturnsAsync(true);
            
            var response = await linkPatientController.LinkPatientCareContexts(consentManagerId, linkRequest) as ObjectResult;
            
            link.Verify();
            discoveryRequestRepository.Verify();
            response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        private async void CheckDataStorageError()
        {
            var faker = Faker();
            var consentManagerId = faker.Random.Hash();
            var linkRequest = GetFakeLinkRequest();
            var expectedError =
                new ErrorRepresentation(new Error(ErrorCode.ServerInternalError, ErrorMessage.DatabaseStorageError));
            link.Setup(e => e.LinkPatients(
                    It.Is<PatientLinkEnquiry>(p =>
                        p.TransactionId == linkRequest.TransactionId &&
                        p.Patient.ReferenceNumber == linkRequest.Patient.ReferenceNumber &&
                        p.Patient.ConsentManagerId == consentManagerId &&
                        p.Patient.ConsentManagerUserId == linkRequest.Patient.ConsentManagerUserId &&
                        Equals(p.Patient.CareContexts, linkRequest.Patient.CareContexts)
                    )
                ))
                .ReturnsAsync(new Tuple<PatientLinkEnquiryRepresentation, ErrorRepresentation>(null, expectedError));
            discoveryRequestRepository.Setup(x => x.RequestExistsFor(linkRequest.TransactionId))
                .ReturnsAsync(true);
            
            var response = await linkPatientController.LinkPatientCareContexts(consentManagerId, linkRequest) as ObjectResult;
            
            link.Verify();
            discoveryRequestRepository.Verify();
            response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        private async void CheckCareContextError()
        {
            var faker = Faker();
            var consentManagerId = faker.Random.Hash();
            var linkRequest = GetFakeLinkRequest();
            var expectedError = new ErrorRepresentation(new Error(ErrorCode.CareContextNotFound,
                "Care context not found for given patient"));
            link.Setup(e => e.LinkPatients(
                    It.Is<PatientLinkEnquiry>(p =>
                        p.TransactionId == linkRequest.TransactionId &&
                        p.Patient.ReferenceNumber == linkRequest.Patient.ReferenceNumber &&
                        p.Patient.ConsentManagerId == consentManagerId &&
                        p.Patient.ConsentManagerUserId == linkRequest.Patient.ConsentManagerUserId &&
                        Equals(p.Patient.CareContexts, linkRequest.Patient.CareContexts)
                    )
                ))
                .ReturnsAsync(new Tuple<PatientLinkEnquiryRepresentation, ErrorRepresentation>(null, expectedError));
            discoveryRequestRepository.Setup(x => x.RequestExistsFor(linkRequest.TransactionId))
                .ReturnsAsync(true);

            var response = await linkPatientController.LinkPatientCareContexts(consentManagerId, linkRequest);

            link.Verify();
            discoveryRequestRepository.Verify();
            response.Should().NotBeNull()
                .And
                .BeOfType<NotFoundObjectResult>()
                .Subject.StatusCode.Should()
                .Be(StatusCodes.Status404NotFound);
        }

        [Fact]
        private async void CheckSearchPatientError()
        {
            var faker = Faker();
            var consentManagerId = faker.Random.Hash();
            var linkRequest = GetFakeLinkRequest();
            var expectedError = new ErrorRepresentation(new Error(ErrorCode.CareContextNotFound, "No patient Found"));
            link.Setup(e => e.LinkPatients(
                    It.Is<PatientLinkEnquiry>(p =>
                        p.TransactionId == linkRequest.TransactionId &&
                        p.Patient.ReferenceNumber == linkRequest.Patient.ReferenceNumber &&
                        p.Patient.ConsentManagerId == consentManagerId &&
                        p.Patient.ConsentManagerUserId == linkRequest.Patient.ConsentManagerUserId &&
                        Equals(p.Patient.CareContexts, linkRequest.Patient.CareContexts)
                    )
                ))
                .ReturnsAsync(new Tuple<PatientLinkEnquiryRepresentation, ErrorRepresentation>(null, expectedError));
            discoveryRequestRepository.Setup(x => x.RequestExistsFor(linkRequest.TransactionId))
                .ReturnsAsync(true);

            var response = await linkPatientController.LinkPatientCareContexts(consentManagerId, linkRequest);

            link.Verify();
            discoveryRequestRepository.Verify();
            response.Should().NotBeNull()
                .And
                .BeOfType<NotFoundObjectResult>()
                .Subject.StatusCode.Should()
                .Be(StatusCodes.Status404NotFound);
        }

        [Fact]
        private async void CheckInvalidOtpError()
        {
            var faker = Faker();
            var consentManagerId = faker.Random.Hash();
            var linkRequest = GetFakeLinkRequest();
            var expectedError = new ErrorRepresentation(new Error(ErrorCode.OtpInValid, "Otp Invalid"));
            link.Setup(e => e.LinkPatients(
                    It.Is<PatientLinkEnquiry>(p =>
                        p.TransactionId == linkRequest.TransactionId &&
                        p.Patient.ReferenceNumber == linkRequest.Patient.ReferenceNumber &&
                        p.Patient.ConsentManagerId == consentManagerId &&
                        p.Patient.ConsentManagerUserId == linkRequest.Patient.ConsentManagerUserId &&
                        Equals(p.Patient.CareContexts, linkRequest.Patient.CareContexts)
                    )
                ))
                .ReturnsAsync(new Tuple<PatientLinkEnquiryRepresentation, ErrorRepresentation>(null, expectedError));
            discoveryRequestRepository.Setup(x => x.RequestExistsFor(linkRequest.TransactionId))
                .ReturnsAsync(true);

            var response = await linkPatientController.LinkPatientCareContexts(consentManagerId, linkRequest);

            link.Verify();
            discoveryRequestRepository.Verify();
            response.Should().NotBeNull()
                .And
                .BeOfType<NotFoundObjectResult>()
                .Subject.StatusCode.Should()
                .Be(StatusCodes.Status404NotFound);
        }

        [Fact]
        private async void CheckTransactionIdNotFoundError()
        {
            var consentManagerId = Faker().Random.Hash();
            var linkRequest = GetFakeLinkRequest();
            var expectedError =
                new ErrorRepresentation(new Error(ErrorCode.DiscoveryRequestNotFound,
                    ErrorMessage.DiscoveryRequestNotFound));
            link.Setup(e => e.LinkPatients(
                    It.Is<PatientLinkEnquiry>(p =>
                        p.TransactionId == linkRequest.TransactionId &&
                        p.Patient.ReferenceNumber == linkRequest.Patient.ReferenceNumber &&
                        p.Patient.ConsentManagerId == consentManagerId &&
                        p.Patient.ConsentManagerUserId == linkRequest.Patient.ConsentManagerUserId &&
                        Equals(p.Patient.CareContexts, linkRequest.Patient.CareContexts))))
                .ReturnsAsync(new Tuple<PatientLinkEnquiryRepresentation, ErrorRepresentation>(null, expectedError));
            discoveryRequestRepository.Setup(x => x.RequestExistsFor(linkRequest.TransactionId))
                .ReturnsAsync(false);
            
            var response = await linkPatientController.LinkPatientCareContexts(consentManagerId, linkRequest) as ObjectResult;
            
            link.Verify();
            discoveryRequestRepository.Verify();
            response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }
    }
}