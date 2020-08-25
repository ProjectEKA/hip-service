using In.ProjectEKA.HipService.Gateway;

namespace In.ProjectEKA.HipServiceTest.Consent
{
    using System;
    using System.Threading.Tasks;
    using Bogus;
    using Builder;
    using FluentAssertions;
    using Hangfire;
    using Hangfire.Common;
    using Hangfire.States;
    using HipService.Common.Model;
    using HipService.Consent;
    using HipService.Consent.Model;
    using In.ProjectEKA.HipService.Gateway.Model;
    using Microsoft.AspNetCore.Http;
    using Moq;
    using Xunit;

    [Collection("Consent Notification Controller Tests")]
    public class ConsentNotificationControllerTest
    {
        private readonly Mock<IConsentRepository> consentRepository;
        private readonly ConsentNotificationController consentNotificationController;
        private readonly Mock<IBackgroundJobClient> backgroundJobClient;
        private readonly Mock<GatewayClient> gatewayClient;
        private ConsentArtefactRepresentation consentNotification;
        private Func<Consent, bool> verifyActualConsentEqualsExpected;

        public ConsentNotificationControllerTest()
        {
            consentRepository = new Mock<IConsentRepository>();
            backgroundJobClient = new Mock<IBackgroundJobClient>();
            gatewayClient = new Mock<GatewayClient>(MockBehavior.Strict, null, null);
            consentNotificationController = new ConsentNotificationController(consentRepository.Object,
                backgroundJobClient.Object,
                gatewayClient.Object);

            SetupConsentNotification(ConsentStatus.GRANTED);

            verifyActualConsentEqualsExpected =
                (actual) =>
                {
                    var expected = consentNotification.Notification;

                    return actual.ConsentArtefactId == expected.ConsentDetail.ConsentId
                        && actual.ConsentArtefact == expected.ConsentDetail
                        && actual.Signature == expected.Signature
                        && actual.Status == expected.Status
                        && actual.ConsentManagerId == expected.ConsentId;
                };

            gatewayClient
                .Setup(g =>
                    g.SendDataToGateway(
                        It.IsAny<string>(),
                        It.IsAny<GatewayRevokedConsentRepresentation>(),
                        It.IsAny<string>()))
                .Returns(Task.Run(() => { }));
        }

        private void SetupConsentNotification(ConsentStatus consentStatus)
        {
            const string consentMangerId = "consentMangerId";
            var notification = TestBuilder.Notification(consentStatus);
            var faker = new Faker();
            consentNotification = new ConsentArtefactRepresentation(notification,
                DateTime.Now,
                faker.Random.Hash());
            var consent =
                new Consent(notification.ConsentDetail.ConsentId,
                    notification.ConsentDetail,
                    notification.Signature,
                    consentStatus,
                    consentMangerId);
            consentRepository.Setup(x => x.AddAsync(consent));
            consentRepository
                .Setup(x => x.GetFor(It.IsAny<string>()))
                .Returns(System.Threading.Tasks.Task.FromResult(consent));
        }

        [Fact]
        private void ShouldEnqueueConsentNotificationAndReturnAccepted()
        {
            var result = consentNotificationController.ConsentNotification(consentNotification);

            backgroundJobClient.Verify(client => client.Create(
                It.Is<Job>(job => job.Method.Name == "StoreConsent" && job.Args[0] == consentNotification),
                It.IsAny<EnqueuedState>()));
            consentRepository.Verify();
            result.StatusCode.Should().Be(StatusCodes.Status202Accepted);
        }

        [Fact]
        async void ShouldStoreConsentArtefact()
        {
            await consentNotificationController.StoreConsent(consentNotification);

            consentRepository.Verify(cr => cr.AddAsync(
                It.Is<Consent>(c => verifyActualConsentEqualsExpected(c))),
                Times.Once);
            consentRepository.Verify(cr =>
                cr.UpdateAsync(It.IsAny<string>(), It.IsAny<ConsentStatus>()), Times.Never);
        }

        [Theory]
        [InlineData(ConsentStatus.DENIED)]
        [InlineData(ConsentStatus.EXPIRED)]
        [InlineData(ConsentStatus.REQUESTED)]
        [InlineData(ConsentStatus.REVOKED)]
        async void ShouldUpdateConsentArtefact(ConsentStatus consentStatus)
        {
            SetupConsentNotification(consentStatus);

            await consentNotificationController.StoreConsent(consentNotification);

            consentRepository.Verify(cr => cr.UpdateAsync(
                    consentNotification.Notification.ConsentId,
                    consentStatus),
                Times.Once);
            consentRepository.Verify(cr => cr.AddAsync(It.IsAny<Consent>()), Times.Never);
        }

        [Fact]
        async void ShouldInvokeGatewayWhenRevokingConsent()
        {
            SetupConsentNotification(ConsentStatus.REVOKED);

            await consentNotificationController.StoreConsent(consentNotification);

            gatewayClient.Verify(g => g.SendDataToGateway(
                        "/v0.5/consents/hip/on-notify",
                        It.Is<GatewayRevokedConsentRepresentation>(
                            c =>
                                c.Acknowledgement.ConsentId == consentNotification.Notification.ConsentId
                                && c.Resp.RequestId == consentNotification.RequestId),
                        consentNotification.Notification.ConsentDetail.ConsentManager.Id
                    ),
                Times.Once);
        }
    }
}