using System.Collections.Generic;
using In.ProjectEKA.HipLibrary.Patient.Model;

namespace In.ProjectEKA.HipService.DataFlow
{
    public class DataResponse
    {
        public string TransactionId { get; }

        public IEnumerable<Entry> Entries { get; }

        public DataResponse(string transactionId, IEnumerable<Entry> entries)
        {
            TransactionId = transactionId;
            Entries = entries;
        }
    }
}