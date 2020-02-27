namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Common;
    using HipLibrary.Patient.Model;
    using Hl7.Fhir.Serialization;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Model;
    using Optional;

    public class DataEntryFactory
    {
        private static readonly FhirJsonSerializer Serializer = new FhirJsonSerializer(new SerializerSettings());
        private static readonly string FhirMediaType = "application/fhir+json";
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IOptions<DataFlowConfiguration> dataFlowConfiguration;
        private readonly IOptions<HipConfiguration> hipConfiguration;
        private const int MbInBytes = 1000000;

        public DataEntryFactory()
        {
        }

        public DataEntryFactory(
            IServiceScopeFactory serviceScopeFactory,
            IOptions<DataFlowConfiguration> dataFlowConfiguration,
            IOptions<HipConfiguration> hipConfiguration)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.dataFlowConfiguration = dataFlowConfiguration;
            this.hipConfiguration = hipConfiguration;
        }

        public virtual Option<IEnumerable<Entry>> Process(Option<Entries> data)
        {
            return data.Map(entries => entries.Bundles.Select(bundle =>
            {
                var serializedBundle = Serializer.SerializeToString(bundle);
                var componentEntry = ComponentEntry(serializedBundle);
                return Linkable(serializedBundle) ? StoreComponentAndGetLink(componentEntry) : componentEntry;
            }));
        }

        private Entry StoreComponentAndGetLink(Entry componentEntry)
        {
            var linkId = Guid.NewGuid().ToString();
            var linkEntry = LinkEntry(linkId);
            StoreComponentEntry(linkId, componentEntry);
            return linkEntry;
        }

        private Entry EntryWith(string content, Link link)
        {
            return new Entry(content, FhirMediaType, "MD5", link);
        }

        private Entry LinkEntry(string linkId)
        {
            var link = LinkFor(linkId);
            return EntryWith(null, link);
        }

        private Entry ComponentEntry(string serializedBundle)
        {
            return EntryWith(serializedBundle, null);
        }

        private bool Linkable(string serializedBundle)
        {
            var byteCount = Encoding.Unicode.GetByteCount(serializedBundle);
            return byteCount >= DataSizeLimitInBytes();
        }

        private int DataSizeLimitInBytes()
        {
            return dataFlowConfiguration.Value.DataSizeLimitInMbs * MbInBytes;
        }

        private Link LinkFor(string linkId)
        {
            var link = $"{hipConfiguration.Value.Url}/health-information/{linkId}";
            return new Link(link);
        }

        private void StoreComponentEntry(string linkId, Entry entry)
        {
            using var serviceScope = serviceScopeFactory.CreateScope();
            var healthInformationRepository = serviceScope.ServiceProvider.GetService<IHealthInformationRepository>();

            var token = Guid.NewGuid().ToString();
            healthInformationRepository.Add(new HealthInformation(linkId, entry, DateTime.Now, token));
        }
    }
}