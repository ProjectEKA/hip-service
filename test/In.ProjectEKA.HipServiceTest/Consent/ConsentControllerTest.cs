namespace In.ProjectEKA.HipServiceTest.Consent
{
    using Builder;
    using HipService.Common.Model;
    using HipService.Consent;
    using HipService.Consent.Model;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using Xunit;

    public class ConsentControllerTest
    {
        private readonly Mock<IConsentRepository> consentRepository = new Mock<IConsentRepository>();

        [Fact]
        async void ShouldStoreConsentArtefact()
        {
            var consentController = new ConsentController(consentRepository.Object);
            const string consentMangerId = "consentMangerId";

            var consentArtefactRequest = TestBuilder.ConsentArtefactRequest();
            consentRepository.Setup(x => x.AddAsync(
                new Consent(consentArtefactRequest.ConsentDetail.ConsentId,
                    consentArtefactRequest.ConsentDetail,
                    consentArtefactRequest.Signature,
                    ConsentStatus.GRANTED,
                    consentMangerId
                )
            ));
            var okObjectResult =
                await consentController.StoreConsent(consentMangerId, consentArtefactRequest) as NoContentResult;

            consentRepository.Verify();
            okObjectResult.StatusCode.Equals(StatusCodes.Status204NoContent);
        }

        [Fact]
        async void ShouldUpdateConsentArtefact()
        {
            var consentController = new ConsentController(consentRepository.Object);
            const string consentMangerId = "consentMangerId";
            var consentArtefactRequest = TestBuilder.ConsentArtefactRequest();
            consentRepository.Setup(x => x.AddAsync(
                new Consent(consentArtefactRequest.ConsentDetail.ConsentId,
                    consentArtefactRequest.ConsentDetail,
                    consentArtefactRequest.Signature,
                    ConsentStatus.GRANTED,
                    consentMangerId)
            ));
            consentRepository.Setup(x => x.UpdateAsync(
                consentArtefactRequest.ConsentId,
                ConsentStatus.REVOKED)
            );
            var okObjectResult =
                await consentController.StoreConsent(consentMangerId, consentArtefactRequest) as NoContentResult;

            consentRepository.Verify();
            okObjectResult.StatusCode.Equals(StatusCodes.Status204NoContent);
        }
        
        [Fact]
        async void ShouldUpdateConsentArtefactAsExpired()
        {
            var consentController = new ConsentController(consentRepository.Object);
            const string consentMangerId = "consentMangerId";
            var consentArtefactRequest = TestBuilder.ConsentArtefactRequest();
            consentRepository.Setup(x => x.AddAsync(
                new Consent(consentArtefactRequest.ConsentDetail.ConsentId,
                    consentArtefactRequest.ConsentDetail,
                    consentArtefactRequest.Signature,
                    ConsentStatus.GRANTED,
                    consentMangerId)
            ));
            consentRepository.Setup(x => x.UpdateAsync(
                consentArtefactRequest.ConsentId,
                ConsentStatus.EXPIRED)
            );
            var okObjectResult =
                await consentController.StoreConsent(consentMangerId, consentArtefactRequest) as NoContentResult;

            consentRepository.Verify();
            okObjectResult.StatusCode.Equals(StatusCodes.Status204NoContent);
        }
    }
}