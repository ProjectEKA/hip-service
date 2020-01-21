using Microsoft.EntityFrameworkCore;

namespace OtpServer.Otp.Model
{
    public class OtpContext:DbContext
    {
        public DbSet<OtpRequest> OtpRequests { get; set; }

        public OtpContext(DbContextOptions options): base(options) 
        {
        }
    }
}