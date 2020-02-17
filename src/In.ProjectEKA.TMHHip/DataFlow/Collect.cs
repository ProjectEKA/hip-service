namespace In.ProjectEKA.TMHHip.DataFlow
{
    using System.Threading.Tasks;
    using HipLibrary.Patient.Model;
    using Optional;

    public class Collect : HipLibrary.Patient.ICollect
    {
        public Task<Option<Entries>> CollectData(DataRequest dataRequest)
        {
            throw new System.NotImplementedException();
        }
    }
}