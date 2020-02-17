namespace In.ProjectEKA.HipService.Consent
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Model;

    [Route("consent")]
    public class ConsentController : Controller
    {
        private readonly IConsentRepository consentRepository;

        public ConsentController(IConsentRepository consentRepository)
        {
            this.consentRepository = consentRepository;
        }

        [HttpPost]
        public async Task<ActionResult> StoreConsent([FromBody] ConsentArtefactRequest consentArtefactRequest)
        {
            var consent = new Consent(consentArtefactRequest.ConsentDetail.ConsentId,
                consentArtefactRequest.ConsentDetail,
                consentArtefactRequest.Signature, consentArtefactRequest.Status);
            await consentRepository.AddAsync(consent);

            return Ok();
        }
    }
}