using System;
using In.ProjectEKA.HipLibrary.Patient.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace In.ProjectEKA.TMHHip.DataFlow.Model
{
    public class MedicationRequestRepresentation:IMedication
    {
        public string FullUrl { get; set; }
        public MedicationRequestResource Resource { get; set; }
    }

    public class MedicationRequestResource
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public HiType ResourceType { get; set; }

        public string Id { get; set; }
        public string Status { get; set; }
        public string Intent { get; set; }
        public Subject Subject { get; set; }
        public MedicationReference MedicationReference { get; set; }
        public DateTime AuthoredOn { get; set; }
        public DosageInstruction DosageInstruction { get; set; }
    }

    public class MedicationReference
    {
        public MedicationReference(string reference, string display)
        {
            Reference = reference;
            Display = display;
        }

        public string Reference { set; get; }
        public string Display { set; get; }
    }

    public class DosageInstruction
    {
        public DosageInstruction(int sequence, string text)
        {
            Sequence = sequence;
            Text = text;
        }

        public string Text { set; get; }
        public int Sequence { get; set; }
    }
}