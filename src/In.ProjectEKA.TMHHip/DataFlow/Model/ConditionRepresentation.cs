using System;
using System.Collections.Generic;
using In.ProjectEKA.HipLibrary.Patient.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace In.ProjectEKA.TMHHip.DataFlow.Model
{
    public class ConditionRepresentation : ICondition
    {
        public string FullUrl { get; set; }
        public ConditionResource Resource { get; set; }
    }

    public class ConditionResource
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public HiType ResourceType { get; set; }

        public string Id { get; set; }
        public Code Code { get; set; }
        public Subject Subject { get; set; }
        public DateTime RecordedDate { get; set; }
        public IEnumerable<Note> Note { get; set; }

        public ConditionResource(HiType resourceType, string id, Code code, Subject subject, DateTime recordedDate,
            IEnumerable<Note> note)
        {
            ResourceType = resourceType;
            Id = id;
            Code = code;
            Subject = subject;
            RecordedDate = recordedDate;
            Note = note;
        }
    }

    public interface ICondition
    {
    }
}