using System.Threading.Tasks;
using In.ProjectEKA.HipLibrary.Patient.Model;
using Optional;

namespace In.ProjectEKA.HipLibrary.Patient
{
    public interface ICollect
    {
        Task<Option<Entries>> CollectData(DataRequest dataRequest);
    }
}