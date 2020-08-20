namespace In.ProjectEKA.FHIRHip.DataFlow.Model
{
    using HipLibrary.Patient.Model;

    public class NetworkData
    {
        public string CareContext { get; set; }
        public HiType HiType { get; set; } 
        public string FHIRData { get; set; }

        public NetworkData(string careContext, HiType hiType, string fhirData)
        {
            CareContext = careContext;
            HiType = hiType;
            FHIRData = fhirData;
        }
    }
}