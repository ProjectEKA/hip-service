namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Mime;
    using System.Text;
    using Common;
    using In.ProjectEKA.HipLibrary.Patient.Model;
    using Model;
    using Logger;
    using Microsoft.Net.Http.Headers;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Task = System.Threading.Tasks.Task;

    public class DataFlowClient
    {
        private readonly HttpClient httpClient;
        private readonly CentralRegistryClient centralRegistryClient;
        private readonly DataFlowNotificationClient dataFlowNotificationClient;
        private readonly CentralRegistryConfiguration centralRegistryConfiguration;

        public DataFlowClient(HttpClient httpClient,
            CentralRegistryClient centralRegistryClient,
            DataFlowNotificationClient dataFlowNotificationClient,
            CentralRegistryConfiguration centralRegistryConfiguration)
        {
            this.httpClient = httpClient;
            this.centralRegistryClient = centralRegistryClient;
            this.dataFlowNotificationClient = dataFlowNotificationClient;
            this.centralRegistryConfiguration = centralRegistryConfiguration;
        }

        public virtual async Task SendDataToHiu(HipLibrary.Patient.Model.DataRequest dataRequest,
            IEnumerable<Entry> data,
            KeyMaterial keyMaterial)
        {
            var url = await centralRegistryClient.GetUrlFor(dataRequest.ConsentManagerId);
            url.MatchSome(async providerUrl => await PostTo(providerUrl,
                dataRequest.ConsentId,
                dataRequest.DataPushUrl,
                dataRequest.CareContexts,
                new DataResponse(dataRequest.TransactionId, data, keyMaterial)));
        }

        private async Task PostTo(string consentMangerUrl,
            string consentId,
            string dataPushUrl,
            IEnumerable<GrantedContext> careContexts,
            DataResponse dataResponse)
        {
            var grantedContexts = careContexts as GrantedContext[] ?? careContexts.ToArray();
            try
            {
                var token = await centralRegistryClient.Authenticate();
                token.MatchSome(async accessToken =>
                {
                    try
                    {
                        await httpClient.SendAsync(CreateHttpRequest(dataPushUrl, dataResponse, accessToken))
                            .ConfigureAwait(false);
                    }
                    catch (Exception exception)
                    {
                        Log.Error(exception, exception.StackTrace);
                        await GetDataNotificationRequest(consentMangerUrl,
                            consentId,
                            grantedContexts,
                            dataResponse,
                            HiStatus.ERRORED,
                            SessionStatus.FAILED,
                            "Failed to deliver health information").ConfigureAwait(false);
                    }
                });
                token.MatchNone(() => Log.Error("Did not post data to HIU"));
                await GetDataNotificationRequest(consentMangerUrl,
                    consentId,
                    grantedContexts,
                    dataResponse,
                    HiStatus.DELIVERED,
                    SessionStatus.TRANSFERRED,
                    "Successfully delivered health information").ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                Log.Error(exception, exception.StackTrace);
            }
        }

        private async Task GetDataNotificationRequest(string consentMangerUrl,
            string consentId,
            IEnumerable<GrantedContext> careContexts,
            DataResponse dataResponse,
            HiStatus hiStatus,
            SessionStatus sessionStatus,
            string description)
        {
            var statusResponses = careContexts
                .Select(grantedContext =>
                    new StatusResponse(grantedContext.CareContextReference, hiStatus, description))
                .ToList();

            await dataFlowNotificationClient.NotifyCm(consentMangerUrl,
                new DataNotificationRequest(dataResponse.TransactionId,
                    DateTime.Now,
                    new Notifier(Type.HIP, centralRegistryConfiguration.ClientId),
                    new StatusNotification(sessionStatus, centralRegistryConfiguration.ClientId, statusResponses),
                    consentId));
        }

        private static HttpRequestMessage CreateHttpRequest<T>(string dataPushUrl, T content, string token)
        {
            var json = JsonConvert.SerializeObject(content, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });
            return new HttpRequestMessage
            {
                RequestUri = new Uri($"{dataPushUrl}"),
                Method = HttpMethod.Post,
                Content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json),
                Headers =
                {
                    {HeaderNames.Authorization, token}
                }
            };
        }
    }
}