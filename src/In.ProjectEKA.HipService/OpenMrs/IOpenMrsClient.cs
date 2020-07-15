using System.Net.Http;
using System.Threading.Tasks;

namespace In.ProjectEKA.HipServiceTest.OpenMrs
{
    public interface IOpenMrsClient
    {
        Task<HttpResponseMessage> GetAsync(string openmrsUrl);
    }
}