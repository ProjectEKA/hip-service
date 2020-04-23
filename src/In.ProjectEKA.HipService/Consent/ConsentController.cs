namespace In.ProjectEKA.HipService.Consent
{
    using System.Threading.Tasks;
    using In.ProjectEKA.HipService.Common.Model;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Model;

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
            [FromHeader(Name = "X-ConsentManagerID")]
            string consentManagerId,
            [FromBody] ConsentArtefactRequest consentArtefactRequest)
        {
            var consent = new Consent(consentArtefactRequest.ConsentDetail.ConsentId,
                consentArtefactRequest.ConsentDetail,
                consentArtefactRequest.Signature,
                consentArtefactRequest.Status,
                consentManagerId);
            if (consentArtefactRequest.Status == ConsentStatus.GRANTED)
            {
                await consentRepository.AddAsync(consent);
            }
            else
            {
                await consentRepository.UpdateAsync(consent);
            }

            return Ok();
        }
    }
}