namespace In.ProjectEKA.TMHHip.DataFlow.Model
{
    using System;

    public class DiagnosticReportAsImage
    {
        private string ReportText { get; set; }
        private DateTime EffectiveDateTime { get; set; }
        private DateTime Issued { get; set; }
        private DateTime StudyStartDate { get; set; }
        private string Performer { get; set; }
        private string ReportConclusion { get; set; }
        private string SubjectReference { get; set; }
        private int NumberOfSeries { get; set; }
        private int NumberOfInstances { get; set; }
        private string PayloadType { get; set; }
        private string PayloadMimeType { get; set; }
        private string ReportUrl { get; set; }

        public DiagnosticReportAsImage(string reportText, DateTime effectiveDateTime, DateTime issued,
            DateTime studyStartDate, string performer, string reportConclusion, string subjectReference,
            int numberOfSeries, int numberOfInstances, string payloadType, string payloadMimeType, string reportUrl)
        {
            ReportText = reportText;
            EffectiveDateTime = effectiveDateTime;
            Issued = issued;
            StudyStartDate = studyStartDate;
            Performer = performer;
            ReportConclusion = reportConclusion;
            SubjectReference = subjectReference;
            NumberOfSeries = numberOfSeries;
            NumberOfInstances = numberOfInstances;
            PayloadType = payloadType;
            PayloadMimeType = payloadMimeType;
            ReportUrl = reportUrl;
        }
    }
}