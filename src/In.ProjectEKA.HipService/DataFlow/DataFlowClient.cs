namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Mime;
    using System.Text;
    using HipLibrary.Patient;
    using Hl7.Fhir.Serialization;
    using Logger;
    using Common;
    using Hl7.Fhir.Model;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Model;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Task = System.Threading.Tasks.Task;

    public class DataFlowClient
    {
        private readonly ICollect collect;
        private readonly HttpClient httpClient;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IOptions<DataFlowConfiguration> dataFlowConfiguration;
        private readonly IOptions<HipConfiguration> hipConfiguration;
        private const int MbInBytes = 1000000;

        private readonly FhirJsonSerializer serializer = new FhirJsonSerializer(new SerializerSettings());

        public DataFlowClient(ICollect collect,
            HttpClient httpClient,
            IServiceScopeFactory serviceScopeFactory,
            IOptions<DataFlowConfiguration> dataFlowConfiguration,
            IOptions<HipConfiguration> hipConfiguration)
        {
            this.collect = collect;
            this.httpClient = httpClient;
            this.serviceScopeFactory = serviceScopeFactory;
            this.dataFlowConfiguration = dataFlowConfiguration;
            this.hipConfiguration = hipConfiguration;
        }

        public async Task HandleMessagingQueueResult(HipLibrary.Patient.Model.DataRequest dataRequest)
        {
            var data = await collect.CollectData(dataRequest);
            data.Map(async entries => await SendDataToHiu(dataRequest, entries.Bundles.Select(EntryFrom).ToList()));
        }

        private async Task SendDataToHiu(
            HipLibrary.Patient.Model.DataRequest dataRequest,
            IEnumerable<Entry> entries)
        {
            await PostTo(dataRequest.CallBackUrl, new DataResponse(dataRequest.TransactionId, entries));
        }

        private Entry EntryFrom(Bundle bundle)
        {
            var serializedBundle = serializer.SerializeToString(bundle);
            var byteCount = Encoding.Unicode.GetByteCount(serializedBundle);
            var componentEntry = ComponentEntry(serializedBundle);
            return IsLink(byteCount) ? StoreComponentAndGetLink(componentEntry) : componentEntry;
        }

        private Entry StoreComponentAndGetLink(Entry componentEntry)
        {
            var linkId = Guid.NewGuid().ToString();
            var linkEntry = LinkEntry(linkId);
            StoreComponentEntry(linkId, componentEntry);
            return linkEntry;
        }

        private Entry LinkEntry(string linkId)
        {
            var link = LinkFor(linkId);
            return EntryWith(null, link);
        }

        private static Entry ComponentEntry(string serializedBundle)
        {
            return EntryWith(serializedBundle, null);
        }

        private bool IsLink(int bundleSize)
        {
            return bundleSize >= DataSizeLimitInBytes();
        }

        private int DataSizeLimitInBytes()
        {
            return dataFlowConfiguration.Value.DataSizeLimitInMbs * MbInBytes;
        }

        private static Entry EntryWith(string content, Link link)
        {
            return new Entry(content, MediaTypeNames.Application.Json, "MD5", link);
        }

        private Link LinkFor(string linkId)
        {
            var link = $"{hipConfiguration.Value.Url}/health-information/{linkId}";
            return new Link(link, "multipart");
        }

        private void StoreComponentEntry(string linkId, Entry entry)
        {
            using var serviceScope = serviceScopeFactory.CreateScope();
            var linkDataRepository = serviceScope.ServiceProvider.GetService<ILinkDataRepository>();

            linkDataRepository.Add(new LinkData(linkId, entry, DateTime.Now, Guid.NewGuid().ToString()));
        }

        private async Task PostTo(string callBackUrl, DataResponse dataResponse)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", GetAuthToken());
                var hiuUrl = $"{callBackUrl}/data/notification";
                await httpClient
                    .PostAsync(hiuUrl, CreateHttpContent(dataResponse))
                    .ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, exception.StackTrace);
            }
        }

        private static string GetAuthToken()
        {
            // TODO : Should be fetched from central registry
            return "AuthToken";
        }

        private static HttpContent CreateHttpContent<T>(T content)
        {
            var json = JsonConvert.SerializeObject(content, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}