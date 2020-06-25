namespace In.ProjectEKA.HipService.Consent
{
    using System.Threading.Tasks;
    using Common.Model;
    using Hangfire;
    using Microsoft.AspNetCore.Mvc;
    using Model;
    using Gateway;

    [ApiController]
    [Route("v1/consents/hip")]
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

        [Route("notify")]
        [HttpPost]
        public AcceptedResult ConsentNotification([FromBody] ConsentArtefactRepresentation consentArtefact)
        {
            backgroundJob.Enqueue(() => StoreConsent(consentArtefact));
            return Accepted();
        }

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
                // if (notification.Status == ConsentStatus.REVOKED)
                // {
                //     var patientId = notification.ConsentDetail.Patient.Id;
                //     var cmSuffix = patientId.Split("@")[1];
                //     var gatewayResponse = new GatewayRevokedConsentRepresentation(
                //         Guid.NewGuid(),
                //         DateTime.Now.ToUniversalTime(), 
                //         new ConsentUpdateResponse(ConsentUpdateStatus.OK.ToString(),
                //             notification.ConsentDetail.ConsentId),
                //         null,
                //         new Resp(consentArtefact.RequestId));
                //     await gatewayClient.SendDataToGateway(ConsentOnNotifyPath, gatewayResponse, cmSuffix);
                // }
            }
        }
    }
}