using System;
using System.Collections.Generic;
using In.ProjectEKA.HipLibrary.Patient.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace In.ProjectEKA.TMHHip.DataFlow.Model
{
    public class ObservationRepresentation : IObservation
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

    public class Member
    {
        public Member(string reference, string display)
        {
            Reference = reference;
            Display = display;
        }

        public string Reference { get; set; }
        public string Display { get; set; }
    }

    public class Note
    {
        public Note(string noteText)
        {
            Text = noteText;
        }

        public string Text { get; set; }
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
        public IEnumerable<Member> HasMember { get; set; }
        public IEnumerable<Note> Note { get; set; }

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

        public Resource(HiType resourceType, string id, string status, Code code, Subject subject,
            DateTime effectiveDateTime, IEnumerable<Member> hasMember)
        {
            ResourceType = resourceType;
            Id = id;
            Status = status;
            Code = code;
            Subject = subject;
            EffectiveDateTime = effectiveDateTime;
            HasMember = hasMember;
        }

        public Resource(HiType resourceType, string id, string status, Code code, Subject subject,
            DateTime effectiveDateTime, string valueString)
        {
            ResourceType = resourceType;
            Id = id;
            Status = status;
            Code = code;
            Subject = subject;
            EffectiveDateTime = effectiveDateTime;
            ValueString = valueString;
        }

        public Resource(HiType resourceType, string id, string status, Code code, Subject subject,
            IEnumerable<Performer> performer, DateTime effectiveDateTime, string valueString, IEnumerable<Note> notes)
        {
            ResourceType = resourceType;
            Id = id;
            Status = status;
            Code = code;
            Subject = subject;
            Performer = performer;
            EffectiveDateTime = effectiveDateTime;
            ValueString = valueString;
            Note = notes;
        }

        public Resource(HiType resourceType, string id, string status, Code code, Subject subject, DateTime effectiveDateTime, string valueString, IEnumerable<Note> note)
        {
            ResourceType = resourceType;
            Id = id;
            Status = status;
            Code = code;
            Subject = subject;
            EffectiveDateTime = effectiveDateTime;
            ValueString = valueString;
            Note = note;
        }
    }

    public class OralCavityExaminationObservationRepresention: IObservation
    {
        public Resource Resource { get; set; }

        public OralCavityExaminationObservationRepresention(Resource resource)
        {
            Resource = resource;
        }
    }

    public interface IObservation
    {
    }
}