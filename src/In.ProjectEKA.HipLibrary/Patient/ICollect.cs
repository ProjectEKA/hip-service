namespace In.ProjectEKA.HipLibrary.Patient
{
    using System.Threading.Tasks;
    using Model;
    using Optional;

    public interface ICollect
    {
        Task<Option<Entries>> CollectData(TraceableDataRequest dataRequest);
    }
}