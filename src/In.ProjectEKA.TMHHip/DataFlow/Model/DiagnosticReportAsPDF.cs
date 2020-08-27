namespace In.ProjectEKA.TMHHip.DataFlow.Model
{
    using System;

    public class DiagnosticReportAsPdf
    {
        private string ReportText { get; set; }
        private DateTime EffectiveDateTime { get; set; }
        private DateTime Issued { get; set; }
        private string Performer { get; set; }
        private string ReportConclusion { get; set; }
        private string ContentType { get; set; }
        private string ReportUrl { get; set; }
        private string ReportTitle { get; set; }

        public DiagnosticReportAsPdf(string reportText, DateTime effectiveDateTime, DateTime issued, string performer,
            string reportConclusion, string contentType, string reportUrl, string reportTitle)
        {
            ReportText = reportText;
            EffectiveDateTime = effectiveDateTime;
            Issued = issued;
            Performer = performer;
            ReportConclusion = reportConclusion;
            ContentType = contentType;
            ReportUrl = reportUrl;
            ReportTitle = reportTitle;
        }
    }
}