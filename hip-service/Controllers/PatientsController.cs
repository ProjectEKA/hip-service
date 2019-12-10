using System;
using System.Collections.Generic;
using hip_library.Patient.models.domain;
using hip_library.Patient.models.dto;
using hip_service.Models.dto;
using hip_service.Services;
using Microsoft.AspNetCore.Mvc;

namespace hip_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly PatientDiscoveryService _patientDiscoveryService;

        public PatientsController(PatientDiscoveryService patientDiscoveryService)
        {
            _patientDiscoveryService = patientDiscoveryService;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<PatientsResponse> Get(PatientRequest request)
        {
            PatientsResponse patientsResponse = new PatientsResponse(_patientDiscoveryService.GetPatients(request));
            return Ok(patientsResponse);
        }
    }
}