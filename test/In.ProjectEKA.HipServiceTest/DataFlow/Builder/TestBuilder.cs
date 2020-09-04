using Hl7.Fhir.Model;

namespace In.ProjectEKA.HipServiceTest.DataFlow.Builder
{
    using System;
    using System.Collections.Generic;
    using Bogus;
    using HipService.DataFlow.Model;
    using In.ProjectEKA.HipService.Common.Model;
    using In.ProjectEKA.HipService.DataFlow;
    using In.ProjectEKA.HipServiceTest.Common.Builder;
    using Moq;
    using Consent = In.ProjectEKA.HipService.Consent.Model.Consent;
    using KeyMaterialLib = HipLibrary.Patient.Model.KeyMaterial;
    using KeyStructureLib = HipLibrary.Patient.Model.KeyStructure;
    using GrantedContext = HipLibrary.Patient.Model.GrantedContext;
    using HiType = HipLibrary.Patient.Model.HiType;
    using Type = In.ProjectEKA.HipService.DataFlow.Type;

    public static class TestBuilder
    {
        private static Faker faker;

        internal static Faker Faker()
        {
            return faker ??= new Faker();
        }

        internal static HealthInformationRequest HealthInformationRequest(string transactionId)
        {
            var time = new TimeSpan(2, 0, 0, 0);
            var expiry = DateTime.Now.Add(time).ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.ffffff");
            return new HealthInformationRequest(
                transactionId,
                new HipService.DataFlow.Consent(faker.Random.Hash()),
                new DateRange(faker.Random.Hash(), faker.Random.Hash()),
                faker.Random.Hash(),
                new KeyMaterial(faker.Random.Word(), faker.Random.Word(),
                    new KeyStructure(expiry, "", faker.Random.Hash()),
                    faker.Random.Hash()));
        }

        private static Faker<ConsentArtefactBuilder> ConsentArtefact()
        {
            return new Faker<ConsentArtefactBuilder>();
        }

        internal static HipLibrary.Patient.Model.DataRequest DataRequest(string transactionId)
        {
            const string consentManagerId = "ConsentManagerId";
            const string consentId = "ConsentId";
            var dateRange = new HipLibrary.Patient.Model.DateRange("from", "to");
            const string callBackUrl = "http://callback/data/notification";
            var keyMaterial = new KeyMaterialLib(faker.Random.Word(), faker.Random.Word(),
                new KeyStructureLib("", "", faker.Random.Hash()),
                faker.Random.Hash());
            return new HipLibrary.Patient.Model.DataRequest(new List<GrantedContext>(),
                dateRange,
                callBackUrl,
                new List<HiType>(),
                transactionId,
                keyMaterial,
                consentManagerId,
                consentId,
                faker.Random.Word());
        }
        
        internal static HipLibrary.Patient.Model.TraceableDataRequest TraceableDataRequest(string transactionId)
        {
            const string consentManagerId = "ConsentManagerId";
            const string consentId = "ConsentId";
            var dateRange = new HipLibrary.Patient.Model.DateRange("from", "to");
            const string callBackUrl = "http://callback/data/notification";
            var keyMaterial = new KeyMaterialLib(faker.Random.Word(), faker.Random.Word(),
                new KeyStructureLib("", "", faker.Random.Hash()),
                faker.Random.Hash());
            return new HipLibrary.Patient.Model.TraceableDataRequest(new List<GrantedContext>(),
                dateRange,
                callBackUrl,
                new List<HiType>(),
                transactionId,
                keyMaterial,
                consentManagerId,
                consentId,
                faker.Random.Word(),
                Uuid.Generate().ToString()
                );
        }

        internal static DataNotificationRequest DataNotificationRequest(string transactionId)
        {
            var notifier = new Notifier(Type.HIP, "10000005");
            const string consentId = "ConsentId";
            var statusNotification = new StatusNotification(
                SessionStatus.TRANSFERRED,
                "10000005",
                new List<StatusResponse>());
            return new DataNotificationRequest(transactionId, DateTime.Now, notifier, statusNotification, consentId, Guid.NewGuid());
        }

        internal static Consent Consent()
        {
            return new Consent(
                faker.Random.Hash(),
                ConsentArtefact().Generate().Build(),
                faker.Random.Hash(),
                ConsentStatus.GRANTED,
                "consentMangerId"
            );
        }

        internal static Consent DataFlowConsent()
        {
            return new Consent(
                faker.Random.Hash(),
                new ConsentArtefact(
                    faker.Random.Word(),
                    faker.Random.Word(),
                    DateTime.Today,
                    new ConsentPurpose(faker.Random.Word(),
                        faker.Random.Word(),
                        faker.Random.Word()),
                    new PatientReference(faker.Random.Word()),
                    new HIPReference(faker.Random.Word(), faker.Random.Word()),
                    new []{HipService.Common.Model.HiType.Condition},
                    new ConsentPermission(AccessMode.View,
                        new AccessPeriod(DateTime.Today,
                            DateTime.Today),
                        new DataFrequency(DataFrequencyUnit.Day, 0, 1),
                        DateTime.Today),
                    new []{new HipService.Common.Model.GrantedContext(faker.Random.Word(),
                        faker.Random.Word())},
                    new OrganizationReference(faker.Random.Word())),
                faker.Random.Word(),
                ConsentStatus.GRANTED,
                faker.Random.Word());
        }

        private static Entry Entry()
        {
            var content = Faker().Random.String();
            var media = Faker().Random.String();
            var checksum = Faker().Random.Hash();
            var careContextReference = Faker().Random.String();
            return new Entry(content, media, checksum, null, careContextReference);
        }

        internal static HealthInformation HealthInformation(string token, DateTime dateTime)
        {
            var linkId = Faker().Random.Hash();
            return new HealthInformation(linkId, Entry(), dateTime, token);
        }

        internal static HealthInformationResponse HealthInformationResponse(string transactionId)
        {
            return new HealthInformationResponse(Entry().Content);
        }

        internal static HipLibrary.Patient.Model.KeyMaterial KeyMaterialLib()
        {
            return new HipLibrary.Patient.Model.KeyMaterial("ECDH", "curve25519",
                new HipLibrary.Patient.Model.KeyStructure(Faker().Random.Word(), Faker().Random.Word(),
                    Faker().Random.Words(32)), Faker().Random.Word());
        }

        internal static KeyMaterial KeyMaterial()
        {
            return new KeyMaterial("ECDH", "curve25519",
                new KeyStructure(Faker().Random.Word(), Faker().Random.Word(),
                    Faker().Random.Words(32)), Faker().Random.Word());
        }

        internal static string RandomString()
        {
            return Faker().Random.String();
        }
    }
}