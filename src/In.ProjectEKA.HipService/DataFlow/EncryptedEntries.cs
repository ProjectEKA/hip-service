namespace In.ProjectEKA.HipService.DataFlow
{
    using System.Collections.Generic;

    public class EncryptedEntries
    {
        public EncryptedEntries(IEnumerable<Entry> entries, KeyMaterial keyMaterial)
        {
            Entries = entries;
            KeyMaterial = keyMaterial;
        }

        public IEnumerable<Entry> Entries { get; }
        public KeyMaterial KeyMaterial { get; }
    }
}