namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    using System.Text.Json.Serialization;
    using System.Xml.Serialization;

    public class Identifier
    {
        public IdentifierType Type { get; }

        public string Value { get; }

        public Identifier(IdentifierType type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}