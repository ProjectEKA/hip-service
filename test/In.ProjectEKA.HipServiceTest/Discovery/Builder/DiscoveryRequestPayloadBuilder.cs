namespace In.ProjectEKA.HipServiceTest.Discovery.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Mime;
    using System.Text;
    using HipLibrary.Patient.Model;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using static Builder.TestBuilders;
    public class DiscoveryRequestPayloadBuilder
    {
        string _requestId;
        private DateTime _requestTime;
        string _transactionId;
        string _patientId;
        string _patientName;
        Gender? _patientGender;
        ushort? _patientYearOfBirth;
        IEnumerable<Identifier> _patientVerifiedIdentifiers;
        IEnumerable<Identifier> _patientUnverifiedIdentifiers;

        public DiscoveryRequestPayloadBuilder WithRequestId()
        {
            _requestId = "3fa85f64 - 5717 - 4562 - b3fc - 2c963f66afa6";
            return this;
        }
        public DiscoveryRequestPayloadBuilder WithRequestId(string requestId)
        {
            _requestId = requestId;
            return this;
        }
        public DiscoveryRequestPayloadBuilder WithTransactionId()
        {
            _transactionId = "4fa85f64 - 5717 - 4562 - b3fc - 2c963f66afa6";
            return this;
        }
        public DiscoveryRequestPayloadBuilder WithTransactionId(string transactionId)
        {
            _transactionId = transactionId;
            return this;
        }
        public DiscoveryRequestPayloadBuilder FromUser(User user)
        {
            return WithPatientId(user.Id)
                .WithPatientName(user.Name)
                .WithPatientGender(user.Gender);
        }
        public DiscoveryRequestPayloadBuilder WithPatientId()
        {
            _patientId = "<patient-id>@<consent-manager-id>";
            return this;
        }
        public DiscoveryRequestPayloadBuilder WithPatientId(string userId)
        {
            _patientId = userId;
            return this;
        }
        public DiscoveryRequestPayloadBuilder WithPatientName()
        {
            _patientName = "chandler bing";
            return this;
        }
        public DiscoveryRequestPayloadBuilder WithPatientName(string userName)
        {
            _patientName = userName;
            return this;
        }
        public DiscoveryRequestPayloadBuilder WithPatientGender()
        {
            _patientGender = Gender.M;
            return this;
        }
        public DiscoveryRequestPayloadBuilder WithPatientGender(Gender? userGender)
        {
            _patientGender = userGender;
            return this;
        }
        public DiscoveryRequestPayloadBuilder WithPatientYearOfBirth(ushort? yearOfBirth)
        {
            _patientYearOfBirth = yearOfBirth;
            return this;
        }
        public DiscoveryRequestPayloadBuilder WithVerifiedIdentifiers(IdentifierType type, string value)
        {
            _patientVerifiedIdentifiers = new List<Identifier> { new Identifier(type, value) };
            return this;
        }
        public DiscoveryRequestPayloadBuilder WithUnverifiedIdentifiers(IdentifierType type, string value)
        {
            _patientUnverifiedIdentifiers = new List<Identifier> { new Identifier(type, value) };
            return this;
        }
        public DiscoveryRequestPayloadBuilder RequestedOn(DateTime requestTime)
        {
            _requestTime = requestTime;
            return this;
        }

        public DiscoveryRequestPayloadBuilder WithMissingParameters(string[] requestParametersToSet)
        {
            WithRequestId();
            WithTransactionId();
            WithPatientId();
            WithPatientName();
            WithPatientGender();

            requestParametersToSet.ToList().ForEach(p =>
            {
                switch (p)
                {
                    case "RequestId": { _requestId = null; break; }
                    case "TransactionId": { _transactionId = null; break; }
                    case "PatientId": { _patientId = null; break; }
                    case "PatientName": { _patientName = null; break; }
                    case "PatientGender": { _patientGender = null; break; }
                    default: throw new ArgumentException("Invalid request parameter name in test", nameof(p));
                }
            });

            return this;
        }

        public DiscoveryRequest Build()
        {
            return new DiscoveryRequest(
                new PatientEnquiry(
                    _patientId, _patientVerifiedIdentifiers, _patientUnverifiedIdentifiers,
                    _patientName, _patientGender, _patientYearOfBirth),
                _requestId,
                _transactionId,
                _requestTime);
        }

        public StringContent BuildSerializedFormat()
        {
            var requestObject = new DiscoveryRequest(
                new PatientEnquiry(
                    _patientId, verifiedIdentifiers: null, unverifiedIdentifiers: null,
                    _patientName, _patientGender, yearOfBirth: null),
                _requestId,
                _transactionId,
                DateTime.Now);
            var json = JsonConvert.SerializeObject(requestObject, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });

            return new StringContent(
                json,
                Encoding.UTF8,
                MediaTypeNames.Application.Json);
        }
    }
}
