namespace In.ProjectEKA.TMHHip.DataFlow.Model
{
    using System.Collections.Generic;
    using HipLibrary.Patient.Model;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class DiagnosticReportEndpointRepresentation : IDiagnosticReport
    {
        public string FullUrl { get; set; }
        public EndpointResource Resource { get; set; }

        public DiagnosticReportEndpointRepresentation(string fullUrl, EndpointResource resource)
        {
            FullUrl = fullUrl;
            Resource = resource;
        }
    }

    public class EndpointResource
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public HiType ResourceType { get; set; }

        public string Id { get; set; }
        public Text Text { get; set; }
        public string Status { get; set; }
        public ConnectionType ConnectionType { get; set; }
        public IEnumerable<PayloadType> PayloadType { get; set; }
        public IEnumerable<string> PayloadMimeType { get; set; }
        public string Address { get; set; }

        public EndpointResource(HiType resourceType, string id, Text text, string status, ConnectionType connectionType,
            IEnumerable<PayloadType> payloadType, IEnumerable<string> payloadMimeType, string address)
        {
            ResourceType = resourceType;
            Id = id;
            Text = text;
            Status = status;
            ConnectionType = connectionType;
            PayloadType = payloadType;
            PayloadMimeType = payloadMimeType;
            Address = address;
        }
    }

    public class PayloadType
    {
        public string Text { get; set; }

        public PayloadType(string text)
        {
            Text = text;
        }
    }

    public class ConnectionType
    {
        public string System { get; set; }
        public string Code { get; set; }

        public ConnectionType(string system, string code)
        {
            System = system;
            Code = code;
        }
    }
}