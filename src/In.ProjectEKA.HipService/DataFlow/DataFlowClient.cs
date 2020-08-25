namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Gateway;
    using HipLibrary.Patient.Model;
    using Logger;
    using Model;
    using static Common.HttpRequestHelper;

    public class DataFlowClient
    {
        private readonly DataFlowNotificationClient dataFlowNotificationClient;
        private readonly GatewayConfiguration gatewayConfiguration;
        private readonly HttpClient httpClient;

        public DataFlowClient(HttpClient httpClient,
            DataFlowNotificationClient dataFlowNotificationClient,
            GatewayConfiguration gatewayConfiguration)
        {
            this.httpClient = httpClient;
            this.dataFlowNotificationClient = dataFlowNotificationClient;
            this.gatewayConfiguration = gatewayConfiguration;
        }

        public virtual async Task SendDataToHiu(HipLibrary.Patient.Model.DataRequest dataRequest,
            IEnumerable<Entry> data,
            KeyMaterial keyMaterial)
        {
            await PostTo(dataRequest.ConsentId,
                dataRequest.DataPushUrl,
                dataRequest.CareContexts,
                new DataResponse(dataRequest.TransactionId, data, keyMaterial),
                dataRequest.CmSuffix).ConfigureAwait(false);
        }

        private async Task PostTo(string consentId,
            string dataPushUrl,
            IEnumerable<GrantedContext> careContexts,
            DataResponse dataResponse,
            string cmSuffix)
        {
            var grantedContexts = careContexts as GrantedContext[] ?? careContexts.ToArray();
            var hiStatus = HiStatus.DELIVERED;
            var sessionStatus = SessionStatus.TRANSFERRED;
            var message = "Successfully delivered health information";
            try
            {
                // TODO: Need to handle non 2xx response also
                await httpClient.SendAsync(CreateHttpRequest(dataPushUrl, dataResponse)).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                hiStatus = HiStatus.ERRORED;
                sessionStatus = SessionStatus.FAILED;
                message = "Failed to deliver health information";
                Log.Error(exception, exception.StackTrace);
            }

            try
            {
                var statusResponses = grantedContexts
                    .Select(grantedContext =>
                        new StatusResponse(grantedContext.CareContextReference, hiStatus,
                            message))
                    .ToList();
                var dataNotificationRequest = new DataNotificationRequest(dataResponse.TransactionId,
                    DateTime.Now.ToUniversalTime(),
                    new Notifier(Type.HIP, gatewayConfiguration.ClientId),
                    new StatusNotification(sessionStatus, gatewayConfiguration.ClientId, statusResponses),
                    consentId,
                    Guid.NewGuid());
                await GetDataNotificationRequest(dataNotificationRequest, cmSuffix).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
        }

        private async Task GetDataNotificationRequest(DataNotificationRequest dataNotificationRequest,
            string cmSuffix)
        {
            await dataFlowNotificationClient.NotifyGateway(cmSuffix, dataNotificationRequest);
        }
    }
}