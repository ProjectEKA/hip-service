using In.ProjectEKA.HipLibrary.Patient.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace In.ProjectEKA.TMHHip.DataFlow.Model
{
    public class MedicationRepresentation
    {
        public string FullUrl { get; set; }
        public MedicationResource Resource { get; set; }
    }

    public class MedicationResource
    {
        [JsonConverter(typeof(StringEnumConverter))]

        public HiType ResourceType { get; set; }

        public string Id { get; set; }
        
        public Code Code { get; set; }
    }
}