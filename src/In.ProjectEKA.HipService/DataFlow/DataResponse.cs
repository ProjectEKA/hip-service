namespace In.ProjectEKA.HipService.DataFlow
{
    using System.Collections.Generic;

    public class DataResponse
    {
        public DataResponse(string transactionId, IEnumerable<Entry> entries, KeyMaterial keyMaterial)
        {
            TransactionId = transactionId;
            Entries = entries;
            KeyMaterial = keyMaterial;
        }

        public string TransactionId { get; }

        public IEnumerable<Entry> Entries { get; }

        public KeyMaterial KeyMaterial { get; }
    }
}