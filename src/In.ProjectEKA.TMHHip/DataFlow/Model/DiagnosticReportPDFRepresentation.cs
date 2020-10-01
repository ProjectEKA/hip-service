namespace In.ProjectEKA.TMHHip.DataFlow.Model
{
    using System;
    using System.Collections.Generic;
    using HipLibrary.Patient.Model;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class DiagnosticReportPdfRepresentation : IDiagnosticReport
    {
        public string FullUrl { get; set; }
        public DiagnosticReportPDFResource Resource { get; set; }

        public DiagnosticReportPdfRepresentation(string fullUrl, DiagnosticReportPDFResource resource)
        {
            FullUrl = fullUrl;
            Resource = resource;
        }
    }

    public class DiagnosticReportPDFResource
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
        public IEnumerable<PresentedForm> PresentedForm { get; set; }
        public string Conclusion { get; set; }

        public DiagnosticReportPDFResource(HiType resourceType, string id, Text text, string status, Code code,
            Subject subject,
            DateTime effectiveDateTime, DateTime issued, IEnumerable<Performer> performer,
            IEnumerable<PresentedForm> presentedForm, string conclusion)
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
            PresentedForm = presentedForm;
            Conclusion = conclusion;
        }
    }

    public class PresentedForm
    {
        public string ContentType { get; set; }
        public string Data { get; set; }
        public string Title { get; set; }

        public PresentedForm(string contentType, string data, string title)
        {
            ContentType = contentType;
            Data = data;
            Title = title;
        }
    }

    public class Text
    {
        public string Status { get; set; }
        public string Div { get; set; }

        public Text(string status, string div)
        {
            Status = status;
            Div = div;
        }
    }
}