namespace In.ProjectEKA.TMHHip.DataFlow.Model
{
    using System.Collections.Generic;
    using HipLibrary.Patient.Model;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class DiagnosticReportEndpointRepresentation : IDiagnosticReport
    {
        private string FullUrl { get; set; }
        private EndpointResource Resource { get; set; }

        public DiagnosticReportEndpointRepresentation(string fullUrl, EndpointResource resource)
        {
            FullUrl = fullUrl;
            Resource = resource;
        }
    }

    public class EndpointResource
    {
        [JsonConverter(typeof(StringEnumConverter))]
        private HiType ResourceType { get; set; }

        private string Id { get; set; }
        private Text Text { get; set; }
        private string Status { get; set; }
        private ConnectionType ConnectionType { get; set; }
        private IEnumerable<PayloadType> PayloadType { get; set; }
        private IEnumerable<string> PayloadMimeType { get; set; }
        private string Address { get; set; }

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
        private string Text { get; set; }

        public PayloadType(string text)
        {
            Text = text;
        }
    }

    public class ConnectionType
    {
        private string System { get; set; }
        private string Code { get; set; }

        public ConnectionType(string system, string code)
        {
            System = system;
            Code = code;
        }
    }
}