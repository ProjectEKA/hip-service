namespace In.ProjectEKA.HipServiceTest.Discovery.Builder
{
    using HipLibrary.Patient.Model;

    internal class IdentifierBuilder
    {
        public IdentifierType Type;

        public string Value;

        public Identifier Build()
        {
            return new Identifier(Type, Value);
        }
    }
}