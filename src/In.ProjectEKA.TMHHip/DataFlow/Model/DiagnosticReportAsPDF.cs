namespace In.ProjectEKA.TMHHip.DataFlow.Model
{
    using System;

    public class DiagnosticReportAsPdf
    {
        public string ReportText { get; set; }
        public DateTime EffectiveDateTime { get; set; }
        public DateTime Issued { get; set; }
        public string Performer { get; set; }
        public string ReportConclusion { get; set; }
        public string ContentType { get; set; }
        public string ReportUrl { get; set; }
        public string ReportTitle { get; set; }
    }
}