namespace In.ProjectEKA.HipService.DataFlow
{
    using System.Threading.Tasks;
    using Model;

    public interface ILinkDataRepository
    {
        void Add(LinkData linkData);
        Task<LinkData> GetAsync(string linkId);
    }
}