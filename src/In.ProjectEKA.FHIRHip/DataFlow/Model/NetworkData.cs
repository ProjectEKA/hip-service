namespace In.ProjectEKA.FHIRHip.DataFlow.Model
{
    using HipLibrary.Patient.Model;

    public class NetworkData
    {
        public string CareContext { get; set; }
        public HiType HiType { get; set; } 
        public string Data { get; set; }

        public NetworkData(string careContext, HiType hiType, string data)
        {
            CareContext = careContext;
            HiType = hiType;
            Data = data;
        }
    }
}