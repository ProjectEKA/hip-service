namespace In.ProjectEKA.HipService.DataFlow
{
    using System.Collections.Generic;
    using HipLibrary.Patient.Model;
    using Optional;

    public interface IDataEntryFactory
    {
        Option<IEnumerable<Entry>> Process(Option<Entries> data);
    }
}