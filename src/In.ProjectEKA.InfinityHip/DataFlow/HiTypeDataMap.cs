namespace In.ProjectEKA.DefaultHip.DataFlow
{
    using System.Collections.Generic;
    using In.ProjectEKA.HipLibrary.Patient.Model;

    public class HiTypeDataMap
    {
        private static readonly List<string> obsList = new List<string>() { "observation.json"};
        private static readonly List<string> diagnosticReportList = new List<string>() { "diagnosticReport.json", "diagnosticReportWithRadiologyImageInline.json"};
        private static readonly Dictionary<HiType, List<string>> Map =
            new Dictionary<HiType, List<string>>
            {
                {HiType.Observation, obsList},
                {HiType.DiagnosticReport, diagnosticReportList}
            };

        public Dictionary<HiType, List<string>> GetMap()
        {
            return Map;
        }
    }
}