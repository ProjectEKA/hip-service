namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Common;
    using Encryptor;
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
        private readonly IEncryptor encryptor;
        private const int MbInBytes = 1000000;

        public DataEntryFactory()
        {
        }

        public DataEntryFactory(
            IServiceScopeFactory serviceScopeFactory,
            IOptions<DataFlowConfiguration> dataFlowConfiguration,
            IOptions<HipConfiguration> hipConfiguration,
            IEncryptor encryptor)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.dataFlowConfiguration = dataFlowConfiguration;
            this.hipConfiguration = hipConfiguration;
            this.encryptor = encryptor;
        }

        public virtual Option<EncryptedEntries> Process(Entries entries,
            HipLibrary.Patient.Model.KeyMaterial dataRequestKeyMaterial, string transactionId)
        {
            var keyPair = EncryptorHelper.GenerateKeyPair(dataRequestKeyMaterial.Curve,
                dataRequestKeyMaterial.CryptoAlg);
            var randomKey = EncryptorHelper.GenerateRandomKey();

            var processedEntries = new List<Entry>();
            foreach (var bundle in entries.Bundles)
            {
                var encryptData = encryptor.EncryptData(
                    dataRequestKeyMaterial,
                    keyPair,
                    Serializer.SerializeToString(bundle),
                    randomKey);
                if (!encryptData.HasValue)
                {
                    return Option.None<EncryptedEntries>();
                }

                encryptData.MatchSome(content =>
                {
                    var entry = IsLinkable(content)
                        ? StoreComponentAndGetLink(ComponentEntry(content), transactionId)
                        : ComponentEntry(content);
                    processedEntries.Add(entry);
                });
            }

            var keyStructure = new KeyStructure(DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"),
                dataRequestKeyMaterial.DhPublicKey.Parameters, EncryptorHelper.GetPublicKey(keyPair));
            var keyMaterial = new KeyMaterial(dataRequestKeyMaterial.CryptoAlg,
                dataRequestKeyMaterial.Curve,
                keyStructure, randomKey);
            return Option.Some(new EncryptedEntries(processedEntries.AsEnumerable(), keyMaterial));
        }

        private Entry StoreComponentAndGetLink(Entry componentEntry, string transactionId)
        {
            var linkId = Guid.NewGuid().ToString();
            var token = Guid.NewGuid().ToString();
            var linkEntry = LinkEntry(linkId, token, transactionId);
            StoreComponentEntry(linkId, componentEntry, token);
            return linkEntry;
        }

        private Entry EntryWith(string content, string link)
        {
            return new Entry(content, FhirMediaType, "MD5", link);
        }

        private Entry LinkEntry(string linkId, string token, string transactionId)
        {
            var link = $"{hipConfiguration.Value.Url}/health-information/{linkId}?token={token}";
            return EntryWith(null, link);
        }

        private Entry ComponentEntry(string serializedBundle)
        {
            return EntryWith(serializedBundle, null);
        }

        private bool IsLinkable(string serializedBundle)
        {
            var byteCount = Encoding.Unicode.GetByteCount(serializedBundle);
            return byteCount >= DataSizeLimitInBytes();
        }

        private int DataSizeLimitInBytes()
        {
            return dataFlowConfiguration.Value.DataSizeLimitInMbs * MbInBytes;
        }

        private void StoreComponentEntry(string linkId, Entry entry, string token)
        {
            using var serviceScope = serviceScopeFactory.CreateScope();
            var healthInformationRepository = serviceScope.ServiceProvider.GetService<IHealthInformationRepository>();

            healthInformationRepository.Add(new HealthInformation(linkId, entry, DateTime.Now, token));
        }
    }
}