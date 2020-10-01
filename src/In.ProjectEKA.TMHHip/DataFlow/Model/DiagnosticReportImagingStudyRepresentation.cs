namespace In.ProjectEKA.TMHHip.DataFlow.Model
{
    using System;
    using System.Collections.Generic;
    using HipLibrary.Patient.Model;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class DiagnosticReportImagingStudyRepresentation : IDiagnosticReport
    {
        public string FullUrl { get; set; }
        public ImagingStudyResource Resource { get; set; }

        public DiagnosticReportImagingStudyRepresentation(string fullUrl, ImagingStudyResource resource)
        {
            FullUrl = fullUrl;
            Resource = resource;
        }
    }

    public class ImagingStudyResource
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public HiType ResourceType { get; set; }

        public string Id { get; set; }
        public Text Text { get; set; }
        public SubjectDiagReport Subject { get; set; }

        public string Status { get; set; }
        public DateTime Started { get; set; }
        public int NumberOfSeries { get; set; }
        public int NumberOfInstances { get; set; }

        public IEnumerable<Endpoint> Endpoint { get; set; }

        public ImagingStudyResource(HiType resourceType, string id, Text text, SubjectDiagReport subject, string status,
            DateTime started, int numberOfSeries, int numberOfInstances, IEnumerable<Endpoint> endpoint)
        {
            ResourceType = resourceType;
            Id = id;
            Text = text;
            Subject = subject;
            Status = status;
            Started = started;
            NumberOfSeries = numberOfSeries;
            NumberOfInstances = numberOfInstances;
            Endpoint = endpoint;
        }
    }

    public class Endpoint
    {
        public string Reference { get; set; }

        public Endpoint(string reference)
        {
            Reference = reference;
        }
    }

    public class SubjectDiagReport
    {
        public string Reference { get; set; }

        public SubjectDiagReport(string reference)
        {
            Reference = reference;
        }
    }
}