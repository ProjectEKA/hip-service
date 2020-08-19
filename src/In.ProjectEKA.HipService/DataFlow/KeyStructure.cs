namespace In.ProjectEKA.HipService.DataFlow
{
    public class KeyStructure
    {
        public KeyStructure(string expiry, string parameters, string keyValue)
        {
            Expiry = expiry;
            Parameters = parameters;
            KeyValue = keyValue;
        }

        public string Expiry { get; }
        public string Parameters { get; }
        public string KeyValue { get; }
    }
}