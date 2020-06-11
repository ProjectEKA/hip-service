namespace In.ProjectEKA.HipService.Consent
{
    using System.Threading.Tasks;
    using Common.Model;
    using Hangfire;
    using Microsoft.AspNetCore.Mvc;
    using Model;

    [Route("v1/consents/hip/notify")]
    public class ConsentNotificationController : ControllerBase
    {
        private readonly IConsentRepository consentRepository;

        private readonly IBackgroundJobClient backgroundJob;

        public ConsentNotificationController(IConsentRepository consentRepository, IBackgroundJobClient backgroundJob)
        {
            this.consentRepository = consentRepository;
            this.backgroundJob = backgroundJob;
        }

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
            }
        }
    }
}