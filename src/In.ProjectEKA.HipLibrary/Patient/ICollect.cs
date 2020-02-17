namespace In.ProjectEKA.HipLibrary.Patient
{
    using System.Threading.Tasks;
    using In.ProjectEKA.HipLibrary.Patient.Model;
    using Optional;

    public interface ICollect
    {
        Task<Option<Entries>> CollectData(DataRequest dataRequest);
    }
}