using System.Threading.Tasks;
using In.ProjectEKA.HipService.Gateway;
using In.ProjectEKA.HipService.Gateway.Model;

namespace In.ProjectEKA.HipServiceTest.Consent
{
    using System;
    using Bogus;
    using Builder;
    using FluentAssertions;
    using Hangfire;
    using Hangfire.Common;
    using Hangfire.States;
    using HipLibrary.Patient.Model;
    using HipService.Common.Model;
    using HipService.Consent;
    using HipService.Consent.Model;
    using HipService.Link;
    using Microsoft.AspNetCore.Http;
    using Moq;
    using Xunit;

    [Collection("Link Controller Tests")]
    public class ConsentNotificationControllerTest
    {
        private readonly Mock<IConsentRepository> consentRepository;
        private readonly ConsentNotificationController consentNotificationController;
        private readonly Mock<IBackgroundJobClient> backgroundJobClient;
        private readonly Mock<GatewayClient> gatewayClient;


        public ConsentNotificationControllerTest()
        {
            consentRepository = new Mock<IConsentRepository>();
            backgroundJobClient = new Mock<IBackgroundJobClient>();
            gatewayClient = new Mock<GatewayClient>(MockBehavior.Strict, null, null, null);
            consentNotificationController = new ConsentNotificationController(consentRepository.Object,
                backgroundJobClient.Object, gatewayClient.Object);
        }

        [Fact]
        private void ShouldEnqueueConsentNotificationAndReturnAccepted()
        {
            const string consentMangerId = "consentMangerId";
            var notification = TestBuilder.Notification();
            var faker = new Faker();
            var consentNotification = new ConsentArtefactRepresentation(notification,
                DateTime.Now,
                faker.Random.Hash());
            consentRepository.Setup(x => x.AddAsync(
                new Consent(notification.ConsentDetail.ConsentId,
                    notification.ConsentDetail,
                    notification.Signature,
                    ConsentStatus.GRANTED,
                    consentMangerId
                )
            ));

            var result = consentNotificationController.ConsentNotification(consentNotification);

            backgroundJobClient.Verify(client => client.Create(
                It.Is<Job>(job => job.Method.Name == "StoreConsent" && job.Args[0] == consentNotification),
                It.IsAny<EnqueuedState>()
            ));
            consentRepository.Verify();
            result.StatusCode.Should().Be(StatusCodes.Status202Accepted);
        }

        // [Fact]
        // private async void ShouldUpdateRevokedConsentAndReturnAccepted()
        // {
        //     var notification = TestBuilder.RevokedNotification("hinapatel@ncg");
        //     var faker = new Faker();
        //     var consentNotification = new ConsentArtefactRepresentation(notification,
        //         DateTime.Now,
        //         faker.Random.Hash());
        //     consentRepository.Setup(x => 
        //         x.UpdateAsync(notification.ConsentId, ConsentStatus.REVOKED));
        //     gatewayClient.Setup(client => client.SendDataToGateway("/v1/consents/hip/on-notify",
        //         It.IsAny<GatewayRevokedConsentRepresentation>(), "ncg")).Returns(Task.CompletedTask);
        //     await consentNotificationController.StoreConsent(consentNotification);
        //     consentRepository.Verify();
        //     gatewayClient.Verify();
        // }
    }
}