
namespace In.ProjectEKA.HipService.Consent
{
    using System;
    using System.Threading.Tasks;
    using Common.Model;
    using Gateway;
    using Gateway.Model;
    using Hangfire;
    using HipLibrary.Patient.Model;
    using Microsoft.AspNetCore.Mvc;
    using Model;
    using static Common.Constants;

    [ApiController]
    [Route(PATH_CONSENTS_HIP)]
    public class ConsentNotificationController : ControllerBase
    {
        private readonly IConsentRepository consentRepository;

        private readonly IBackgroundJobClient backgroundJob;

        private readonly GatewayClient gatewayClient;

        public ConsentNotificationController(
            IConsentRepository consentRepository,
            IBackgroundJobClient backgroundJob,
            GatewayClient gatewayClient)
        {
            this.consentRepository = consentRepository;
            this.backgroundJob = backgroundJob;
            this.gatewayClient = gatewayClient;
        }

        [HttpPost]
        public AcceptedResult ConsentNotification([FromBody] ConsentArtefactRepresentation consentArtefact)
        {
            backgroundJob.Enqueue(() => StoreConsent(consentArtefact));
            return Accepted();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task StoreConsent(ConsentArtefactRepresentation consentArtefact)
        {
            var notification = consentArtefact.Notification;

            if (notification.Status == ConsentStatus.GRANTED)
            {
                var consent = new Consent(notification.ConsentDetail.ConsentId,
                    notification.ConsentDetail,
                    notification.Signature,
                    notification.Status,
                    notification.ConsentId);
                await consentRepository.AddAsync(consent);
            }
            else
            {
                await consentRepository.UpdateAsync(notification.ConsentId, notification.Status);
                if (notification.Status == ConsentStatus.REVOKED)
                {
                    var consent = await consentRepository.GetFor(notification.ConsentId);
                    var cmSuffix = consent.ConsentArtefact.ConsentManager.Id;
                    var gatewayResponse = new GatewayRevokedConsentRepresentation(
                        Guid.NewGuid(),
                        DateTime.Now.ToUniversalTime(), 
                        new ConsentUpdateResponse(ConsentUpdateStatus.OK.ToString(),
                            notification.ConsentId),
                        null,
                        new Resp(consentArtefact.RequestId));
                    await gatewayClient.SendDataToGateway(PATH_CONSENT_ON_NOTIFY, gatewayResponse, cmSuffix);
                }
            }
        }
    }
}
