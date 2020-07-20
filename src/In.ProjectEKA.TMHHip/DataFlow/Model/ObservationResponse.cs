using System.Collections.Generic;

namespace In.ProjectEKA.TMHHip.DataFlow.Model
{
    public class ObservationResponse
    {
        public string ResourceType { get; set; }
        public string Id { get; set; }
        public string Type { get; set; }
        public IEnumerable<IObservation> Entry { get; set; }
    }

    public class MedicationResponse
    {
        public string ResourceType { get; set; }
        public string Id { get; set; }
        public string Type { get; set; }
        public IEnumerable<IMedication> Entry { get; set; }
    }

    public class ConditionResponse
    {
        public string ResourceType { get; set; }
        public string Id { get; set; }
        public string Type { get; set; }
        public IEnumerable<ICondition> Entry { get; set; }
    }
}