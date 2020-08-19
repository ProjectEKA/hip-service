namespace In.ProjectEKA.HipService.Consent
{
    using System;
    using System.Threading.Tasks;
    using Common.Model;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Model;

    [Obsolete]
    [Authorize]
    [Route("consent/notification")]
    public class ConsentController : Controller
    {
        private readonly IConsentRepository consentRepository;

        public ConsentController(IConsentRepository consentRepository)
        {
            this.consentRepository = consentRepository;
        }

        [HttpPost]
        public async Task<ActionResult> StoreConsent(
            [FromHeader(Name = "X-GatewayID ")] string consentManagerId,
            [FromBody] ConsentArtefactRequest consentArtefactRequest)
        {
            if (consentArtefactRequest.Status == ConsentStatus.GRANTED)
            {
                var consent = new Consent(consentArtefactRequest.ConsentDetail.ConsentId,
                    consentArtefactRequest.ConsentDetail,
                    consentArtefactRequest.Signature,
                    consentArtefactRequest.Status,
                    consentManagerId);
                await consentRepository.AddAsync(consent);
            }
            else
            {
                await consentRepository.UpdateAsync(consentArtefactRequest.ConsentId, consentArtefactRequest.Status);
            }

            return NoContent();
        }
    }
}