namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public class KeyStructure
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public string Expiry { get; }
        public string Parameters { get; }

        public string KeyValue { get; }

        public KeyStructure(string expiry, string parameters, string keyValue)
        {
            Expiry = expiry;
            Parameters = parameters;
            KeyValue = keyValue;
        }
    }
}