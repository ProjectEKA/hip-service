namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Common;
    using Logger;
    using Model;
    using static Common.HttpRequestHelper;

    public class DataFlowNotificationClient
    {
        private readonly HttpClient httpClient;
        private readonly CentralRegistryClient centralRegistryClient;
        private static readonly string HealthInformationNotificationPath = "/health-information/notification";


        public DataFlowNotificationClient(HttpClient httpClient, CentralRegistryClient centralRegistryClient)
        {
            this.httpClient = httpClient;
            this.centralRegistryClient = centralRegistryClient;
        }

        public virtual async Task NotifyCm(string url, DataNotificationRequest dataNotificationRequest)
        {
            await PostTo(url, dataNotificationRequest);
        }

        private async Task PostTo(string url, DataNotificationRequest dataNotificationRequest)
        {
            try
            {
                var token = await centralRegistryClient.Authenticate();
                token.MatchSome(async accessToken =>
                {
                    try
                    {
                        await httpClient
                            .SendAsync(CreateHttpRequest(url + HealthInformationNotificationPath,
                                dataNotificationRequest,
                                accessToken))
                            .ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Log.Fatal(ex, "Error happened");
                    }
                });
                token.MatchNone(() => Log.Information("Data transfer notification to CM failed"));
            }
            catch (Exception exception)
            {
                Log.Error(exception, exception.StackTrace);
            }
        }
    }
}