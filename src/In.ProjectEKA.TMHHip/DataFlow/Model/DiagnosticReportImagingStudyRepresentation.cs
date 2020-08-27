namespace In.ProjectEKA.TMHHip.DataFlow.Model
{
    using System;
    using System.Collections.Generic;
    using HipLibrary.Patient.Model;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class DiagnosticReportImagingStudyRepresentation : IDiagnosticReport
    {
        private string FullUrl { get; set; }
        private ImagingStudyResource Resource { get; set; }

        public DiagnosticReportImagingStudyRepresentation(string fullUrl, ImagingStudyResource resource)
        {
            FullUrl = fullUrl;
            Resource = resource;
        }
    }

    public class ImagingStudyResource
    {
        [JsonConverter(typeof(StringEnumConverter))]
        private HiType ResourceType { get; set; }

        private string Id { get; set; }
        private Text Text { get; set; }
        private SubjectDiagReport Subject { get; set; }

        private string Status { get; set; }
        private DateTime Started { get; set; }
        private int NumberOfSeries { get; set; }
        private int NumberOfInstances { get; set; }

        private IEnumerable<Endpoint> Endpoint { get; set; }

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
        private string Reference { get; set; }

        public Endpoint(string reference)
        {
            Reference = reference;
        }
    }

    public class SubjectDiagReport
    {
        private string Reference { get; set; }

        public SubjectDiagReport(string reference)
        {
            Reference = reference;
        }
    }
}