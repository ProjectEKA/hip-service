namespace In.ProjectEKA.DefaultHip.DataFlow 
{
    using System.Collections.Generic;

    public class PatientCCRecord
    {
        public string capturedOn { get; set; }
        
        public Dictionary<string, List<string>> data { get; set; }
    }

}
