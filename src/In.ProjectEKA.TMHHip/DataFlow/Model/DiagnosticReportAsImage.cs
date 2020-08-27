namespace In.ProjectEKA.TMHHip.DataFlow.Model
{
    using System;

    public class DiagnosticReportAsImage
    {
        public string ReportText { get; set; }
        public DateTime EffectiveDateTime { get; set; }
        public DateTime Issued { get; set; }
        public DateTime StudyStartDate { get; set; }
        public string Performer { get; set; }
        public string ReportConclusion { get; set; }
        public string SubjectReference { get; set; }
        public int NumberOfSeries { get; set; }
        public int NumberOfInstances { get; set; }
        public string PayloadType { get; set; }
        public string PayloadMimeType { get; set; }
        public string ReportUrl { get; set; }
    }
}