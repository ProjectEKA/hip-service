namespace In.ProjectEKA.DefaultHip.DataFlow
{
    using System.Collections.Generic;
    using In.ProjectEKA.HipLibrary.Patient.Model;

    public class HiTypeDataMap
    {
        private static readonly List<string> ObsList
            = new List<string>
            {
                "observation.json",
                "ShriyaTMHFirstVisitConditionAndObs.json",
                "ShriyaTMHThirdVisitObsWithRef.json"
            };

        private static readonly List<string> DiagnosticReportList
            = new List<string>
            {
                "diagnosticReport.json",
                "diagnosticReportWithRadiologyImageInline.json",
                "dignosticReportWithMediaDicomFileAsUrl.json",
                "dignosticReportWithPresentedFormPdfFileAsUrl.json",
                "ShriyaTMHFirstVisitDiagnosticReport.json",
                "ShriyaTMHThirdVisitDiagnosticReports.json"
            };

        private static readonly List<string> MedicationRequestList
            = new List<string>
            {
                "ShriyaTMHThirdVisitMedication.json"
            };

        private static readonly Dictionary<HiType, List<string>> Map =
            new Dictionary<HiType, List<string>>
            {
                {HiType.Observation, ObsList},
                {HiType.DiagnosticReport, DiagnosticReportList},
                {HiType.MedicationRequest, MedicationRequestList}
            };

        public Dictionary<HiType, List<string>> GetMap()
        {
            return Map;
        }
    }
}