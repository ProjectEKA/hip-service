namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public class Identifier
    {
        public Identifier(IdentifierType type, string value)
        {
            Type = type;
            Value = value;
        }

        public IdentifierType Type { get; }

        public string Value { get; }
    }
}