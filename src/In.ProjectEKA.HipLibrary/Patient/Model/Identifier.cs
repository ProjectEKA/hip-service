namespace In.ProjectEKA.HipLibrary.Patient.Model
{
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