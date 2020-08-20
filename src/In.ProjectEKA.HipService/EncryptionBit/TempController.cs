using Microsoft.AspNetCore.Mvc;

namespace In.ProjectEKA.HipService.EncryptionBit
{
    [ApiController]
    public class TempController : ControllerBase
    {
        private readonly EncrytorDemo encrytorDemo;

        public TempController(EncrytorDemo encrytorDemo)
        {
            this.encrytorDemo = encrytorDemo;
        }

        [HttpPost("encrypt")]
        public AcceptedResult DiscoverPatientCareContexts()
        {
            encrytorDemo.SetUpDemo();
            return null;
        }
       
    }
}