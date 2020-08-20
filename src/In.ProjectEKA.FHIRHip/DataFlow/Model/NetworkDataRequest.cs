namespace In.ProjectEKA.DefaultHip.DataFlow.Model
{
    using System.Collections.Generic;
    using HipLibrary.Patient.Model;

    public class NetworkDataRequest
    {
        public string PatientReference { get; set; }
        public IEnumerable<string> CareContexts { get; set; }
        public DateRange DataRange { get; set; }
        public IEnumerable<HiType> HiTypes { get; set; }

        public NetworkDataRequest(string patientReference, IEnumerable<string> careContexts, DateRange dataRange, IEnumerable<HiType> hiTypes)
        {
            PatientReference = patientReference;
            CareContexts = careContexts;
            DataRange = dataRange;
            HiTypes = hiTypes;
        }
    }
}