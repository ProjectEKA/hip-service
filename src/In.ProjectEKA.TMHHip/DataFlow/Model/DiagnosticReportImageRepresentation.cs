namespace In.ProjectEKA.TMHHip.DataFlow.Model
{
    using System;
    using System.Collections.Generic;
    using HipLibrary.Patient.Model;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class DiagnosticReportImageRepresentation : IDiagnosticReport
    {
        private string FullUrl { get; set; }
        private DiagnosticReportImage Resource { get; set; }

        public DiagnosticReportImageRepresentation(string fullUrl, DiagnosticReportImage resource)
        {
            FullUrl = fullUrl;
            Resource = resource;
        }
    }

    public class DiagnosticReportImage
    {
        [JsonConverter(typeof(StringEnumConverter))]
        private HiType ResourceType { get; set; }

        private string Id { get; set; }
        private Text Text { get; set; }
        private string Status { get; set; }
        private Code Code { get; set; }
        private Subject Subject { get; set; }
        private DateTime EffectiveDateTime { get; set; }
        private DateTime Issued { get; set; }
        private IEnumerable<Performer> Performer { get; set; }
        private IEnumerable<DiagnosticReportImagingStudy> ImagingStudy { get; set; }
        private string Conclusion { get; set; }

        public DiagnosticReportImage(HiType resourceType, string id, Text text, string status, Code code,
            Subject subject, DateTime effectiveDateTime, DateTime issued, IEnumerable<Performer> performer,
            IEnumerable<DiagnosticReportImagingStudy> imagingStudy, string conclusion)
        {
            ResourceType = resourceType;
            Id = id;
            Text = text;
            Status = status;
            Code = code;
            Subject = subject;
            EffectiveDateTime = effectiveDateTime;
            Issued = issued;
            Performer = performer;
            ImagingStudy = imagingStudy;
            Conclusion = conclusion;
        }
    }

    public class DiagnosticReportImagingStudy
    {
        private string Reference { get; set; }

        public DiagnosticReportImagingStudy(string reference)
        {
            Reference = reference;
        }
    }

    public interface IDiagnosticReport
    {
    }
}