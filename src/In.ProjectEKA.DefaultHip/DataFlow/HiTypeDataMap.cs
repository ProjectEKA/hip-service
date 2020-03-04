namespace In.ProjectEKA.DefaultHip.DataFlow
{
    using System.Collections.Generic;
    using In.ProjectEKA.HipLibrary.Patient.Model;

    public class HiTypeDataMap
    {
        private static readonly Dictionary<HiType, string> Map =
            new Dictionary<HiType, string>
            {
                {HiType.Observation, "observation.json"},
                {HiType.DiagnosticReport, "diagnosticReport.json"}
            };

        public Dictionary<HiType, string> GetMap()
        {
            return Map;
        }
    }
}