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

        public virtual async void SendDataToHiu(HipLibrary.Patient.Model.DataRequest dataRequest,
            IEnumerable<Entry> data,
            KeyMaterial keyMaterial)
        {
            var url = await centralRegistryClient.GetUrlFor(dataRequest.ConsentManagerId);
            url.MatchSome(async providerUrl => await PostTo(providerUrl,
                dataRequest.CallBackUrl,
                dataRequest.CareContexts,
                new DataResponse(dataRequest.TransactionId, data, keyMaterial)));
        }

        private async Task PostTo(string url,
            string callBackUrl,
            IEnumerable<GrantedContext> careContexts,
            DataResponse dataResponse)
        {
            var grantedContexts = careContexts as GrantedContext[] ?? careContexts.ToArray();
            try
            {
                var token = await centralRegistryClient.Authenticate();
                token.MatchSome(async accessToken => await httpClient
                    .SendAsync(CreateHttpRequest(callBackUrl, dataResponse, accessToken))
                    .ConfigureAwait(false));
                token.MatchNone(() => Log.Information("Did not post data to HIU"));
                GetDataNotificationRequest(url,
                    grantedContexts,
                    dataResponse,
                    HiStatus.DELIVERED,
                    SessionStatus.TRANSFERRED,
                    "Successfully delivered health information");
            }
            catch (Exception exception)
            {
                Log.Error(exception, exception.StackTrace);
                GetDataNotificationRequest(url,
                    grantedContexts,
                    dataResponse,
                    HiStatus.ERRORED,
                    SessionStatus.FAILED,
                    "Failed to deliver health information");
            }
        }

        private void GetDataNotificationRequest(string url,
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

            dataFlowNotificationClient.NotifyCm(url,
                new DataNotificationRequest(dataResponse.TransactionId,
                    DateTime.Now,
                    new Notifier(Type.HIP, centralRegistryConfiguration.ClientId),
                    new StatusNotification(sessionStatus, centralRegistryConfiguration.ClientId, statusResponses)));
        }

        private static HttpRequestMessage CreateHttpRequest<T>(string callBackUrl, T content, string token)
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
                RequestUri = new Uri($"{callBackUrl}/data/notification"),
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