﻿namespace In.ProjectEKA.HipService.Discovery
{
    using System.Threading.Tasks;
    using HipLibrary.Patient.Model;
    using Logger;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    [Route("patients/discover/carecontexts")]
    [ApiController]
    public class DiscoveryController : Controller
    {
        private readonly PatientDiscovery patientDiscovery;

        public DiscoveryController(PatientDiscovery patientDiscovery)
        {
            this.patientDiscovery = patientDiscovery;
        }

        [HttpPost]
        public async Task<ActionResult> Discover(DiscoveryRequest request)
        {
            var (discoveryResponse, error) = await patientDiscovery.PatientFor(request);

            if (error != null) return NotFound(error);

            return Ok(discoveryResponse);
        }
    }
}