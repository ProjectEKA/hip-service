namespace In.ProjectEKA.HipService.DataFlow
{
    using System.Collections.Generic;
    using Optional;

    public interface IDataFlowClient
    {
        void SendDataToHiu(HipLibrary.Patient.Model.DataRequest dataRequest, Option<IEnumerable<Entry>> data);
    }
}