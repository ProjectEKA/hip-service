namespace In.ProjectEKA.TMHHip.DataFlow.Model
{
    using System;
    using System.Collections.Generic;
    using HipLibrary.Patient.Model;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class DiagnosticReportPdfRepresentation:IDiagnosticReport
    {
        private string FullUrl { get; set; }
        private DiagnosticReportPDFResource Resource { get; set; }

        public DiagnosticReportPdfRepresentation(string fullUrl, DiagnosticReportPDFResource resource)
        {
            FullUrl = fullUrl;
            Resource = resource;
        }
    }

    public class DiagnosticReportPDFResource
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
        private IEnumerable<PresentedForm> PresentedForm { get; set; }
        private string Conclusion { get; set; }

        public DiagnosticReportPDFResource(HiType resourceType, string id, Text text, string status, Code code, Subject subject,
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
        private string ContentType { get; set; }
        private string Data { get; set; }
        private string Title { get; set; }

        public PresentedForm(string contentType, string data, string title)
        {
            ContentType = contentType;
            Data = data;
            Title = title;
        }
    }

    public class Text
    {
        private string Status { get; set; }
        private string Div { get; set; }

        public Text(string status, string div)
        {
            Status = status;
            Div = div;
        }
    }
}