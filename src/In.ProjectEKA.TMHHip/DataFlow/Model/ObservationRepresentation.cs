using System;
using System.Collections.Generic;
using Hl7.Fhir.Model;
using In.ProjectEKA.HipLibrary.Patient.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace In.ProjectEKA.TMHHip.DataFlow.Model
{
    public class ObservationRepresentation
    {
        public string FullUrl { get; set; }
        public Resource Resource { get; set; }
    }

    public class Performer
    {
        public Performer(string display)
        {
            Display = display;
        }

        public string Display { get; set; }
    }

    public class Resource
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public HiType ResourceType { get; set; }

        public string Id { get; set; }
        public string Status { get; set; }
        public Code Code { get; set; }
        public Subject Subject { get; set; }
        public IEnumerable<Performer> Performer { get; set; }
        public DateTime EffectiveDateTime { get; set; }
        public string ValueString { get; set; }

        public Resource(HiType resourceType, string id, string status, Code code, Subject subject,
            IEnumerable<Performer> performer, DateTime effectiveDateTime, string valueString)
        {
            ResourceType = resourceType;
            Id = id;
            Status = status;
            Code = code;
            Subject = subject;
            Performer = performer;
            EffectiveDateTime = effectiveDateTime;
            ValueString = valueString;
        }
    }
}