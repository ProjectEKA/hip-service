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

            var consentArtefactRequest = TestBuilder.ConsentArtefactRequest();
            consentRepository.Setup(x => x.AddAsync(
                new Consent(consentArtefactRequest.ConsentDetail.ConsentId,
                    consentArtefactRequest.ConsentDetail,
                    consentArtefactRequest.Signature,
                    ConsentStatus.GRANTED)
            ));
            var okObjectResult = await consentController.StoreConsent(consentArtefactRequest) as OkResult;

            consentRepository.Verify();
            okObjectResult.StatusCode.Equals(StatusCodes.Status200OK);
        }

        [Fact]
        async void ShouldUpdateConsentArtefact()
        {
            var consentController = new ConsentController(consentRepository.Object);

            var consentArtefactRequest = TestBuilder.ConsentArtefactRequest();
            consentRepository.Setup(x => x.AddAsync(
                new Consent(consentArtefactRequest.ConsentDetail.ConsentId,
                    consentArtefactRequest.ConsentDetail,
                    consentArtefactRequest.Signature,
                    ConsentStatus.GRANTED)
            ));
            consentRepository.Setup(x => x.UpdateAsync(
                new Consent(consentArtefactRequest.ConsentDetail.ConsentId,
                    consentArtefactRequest.ConsentDetail,
                    consentArtefactRequest.Signature,
                    ConsentStatus.REVOKED)
            ));
            var okObjectResult = await consentController.StoreConsent(consentArtefactRequest) as OkResult;

            consentRepository.Verify();
            okObjectResult.StatusCode.Equals(StatusCodes.Status200OK);
        }
    }
}