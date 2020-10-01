namespace In.ProjectEKA.TMHHip.DataFlow.Model
{
    using System;
    using System.Collections.Generic;
    using HipLibrary.Patient.Model;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class DiagnosticReportImageRepresentation : IDiagnosticReport
    {
        public string FullUrl { get; set; }
        public DiagnosticReportImage Resource { get; set; }

        public DiagnosticReportImageRepresentation(string fullUrl, DiagnosticReportImage resource)
        {
            FullUrl = fullUrl;
            Resource = resource;
        }
    }

    public class DiagnosticReportImage
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public HiType ResourceType { get; set; }

        public string Id { get; set; }
        public Text Text { get; set; }
        public string Status { get; set; }
        public Code Code { get; set; }
        public Subject Subject { get; set; }
        public DateTime EffectiveDateTime { get; set; }
        public DateTime Issued { get; set; }
        public IEnumerable<Performer> Performer { get; set; }
        public IEnumerable<DiagnosticReportImagingStudy> ImagingStudy { get; set; }
        public string Conclusion { get; set; }

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
        public string Reference { get; set; }

        public DiagnosticReportImagingStudy(string reference)
        {
            Reference = reference;
        }
    }

    public interface IDiagnosticReport
    {
    }
}