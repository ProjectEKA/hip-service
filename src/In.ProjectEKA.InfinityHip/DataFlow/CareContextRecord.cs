namespace In.ProjectEKA.DefaultHip.DataFlow
{
    using System.Collections.Generic;

    public class CareContextRecord
    {
        public string CapturedOn { get; set; }

        public Dictionary<string, List<string>> Data { get; set; }
    }
}