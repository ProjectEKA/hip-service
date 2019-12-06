using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace hip_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] {};
        }
    }
}