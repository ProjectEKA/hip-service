namespace hip_service_test.Discovery
{
    using HipLibrary.Patient.Models;

    internal class IdentifierBuilder
    {
        public IdentifierType Type;

        public string Value;

        public Identifier build()
        {
            return new Identifier(Type, Value);
        }
    }
}