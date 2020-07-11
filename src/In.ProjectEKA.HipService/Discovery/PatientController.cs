using static In.ProjectEKA.HipService.Common.Constants;

namespace In.ProjectEKA.HipService.Discovery
{
    using System;
    using System.Threading.Tasks;
    using Gateway;
    using Gateway.Model;
    using Hangfire;
    using HipLibrary.Patient.Model;
    using Logger;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    [ApiController]
    public class CareContextDiscoveryController : Controller
    {
        private readonly PatientDiscovery patientDiscovery;
        private readonly GatewayClient gatewayClient;
        private readonly IBackgroundJobClient backgroundJob;

        public CareContextDiscoveryController(PatientDiscovery patientDiscovery,
            GatewayClient gatewayClient,
            IBackgroundJobClient backgroundJob)
        {
            this.patientDiscovery = patientDiscovery;
            this.gatewayClient = gatewayClient;
            this.backgroundJob = backgroundJob;
        }

        [HttpPost(PATH_CARE_CONTEXTS_DISCOVER)]
        public AcceptedResult DiscoverPatientCareContexts(DiscoveryRequest request)
        {
            backgroundJob.Enqueue(() => GetPatientCareContext(request));
            return Accepted();
        }

        private async Task GetPatientCareContext(DiscoveryRequest request)
        {
            try
            {
                var (response, error) = await patientDiscovery.PatientFor(request);
                var patientId = request.Patient.Id;
                var cmSuffix = patientId.Substring(
                    patientId.LastIndexOf("@", StringComparison.Ordinal) + 1);

                var gatewayDiscoveryRepresentation = new GatewayDiscoveryRepresentation(
                    response?.Patient,
                    Guid.NewGuid(),
                    DateTime.Now.ToUniversalTime(),
                    request.TransactionId, //TODO: should be reading transactionId from contract
                    error?.Error,
                    new Resp(request.RequestId));
                await gatewayClient.SendDataToGateway(PATH_ON_DISCOVER, gatewayDiscoveryRepresentation, cmSuffix);
            }
            catch (Exception exception)
            {
                Log.Error(exception, exception.StackTrace);
            }
        }
    }
}