namespace In.ProjectEKA.OtpService.Otp
{
    using System.Threading.Tasks;
    using Common;
    using Model;
    using Optional;
    
    public interface IOtpRepository
    {
        Task<Response> Save(string otp, string sessionId);
        
        Task<Option<OtpRequest>> GetWith(string sessionId);
    }
}