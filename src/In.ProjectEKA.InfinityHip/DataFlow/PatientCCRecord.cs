namespace In.ProjectEKA.DefaultHip.DataFlow
{
    using System.Collections.Generic;

    public class PatientCcRecord
    {
        public string CapturedOn { get; set; }

        public Dictionary<string, List<string>> Data { get; set; }
    }
}