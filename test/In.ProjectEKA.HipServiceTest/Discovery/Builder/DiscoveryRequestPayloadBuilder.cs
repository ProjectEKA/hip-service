namespace In.ProjectEKA.HipServiceTest.Discovery.Builder
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Mime;
    using System.Text;
    using In.ProjectEKA.HipLibrary.Patient.Model;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class DiscoveryRequestPayloadBuilder
    {
        string _requestId;
        string _transactionId;
        string _patientId;
        string _patientName;
        Gender? _patientGender;

        public DiscoveryRequestPayloadBuilder WithRequestId()
        {
            _requestId = "3fa85f64 - 5717 - 4562 - b3fc - 2c963f66afa6";
            return this;
        }
        public DiscoveryRequestPayloadBuilder WithTransactionId()
        {
            _transactionId = "4fa85f64 - 5717 - 4562 - b3fc - 2c963f66afa6";
            return this;
        }
        public DiscoveryRequestPayloadBuilder WithPatientId()
        {
            _patientId = "<patient-id>@<consent-manager-id>";
            return this;
        }
        public DiscoveryRequestPayloadBuilder WithPatientName()
        {
            _patientName = "chandler bing";
            return this;
        }
        public DiscoveryRequestPayloadBuilder WithPatientGender()
        {
            _patientGender = Gender.M;
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

        public StringContent Build()
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
