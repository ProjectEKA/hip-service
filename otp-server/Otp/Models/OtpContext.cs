using Microsoft.EntityFrameworkCore;

namespace otp_server.Otp.Models
{
    public class OtpContext:DbContext
    {
        public DbSet<OtpRequest> OtpRequests { get; set; }

        public OtpContext(DbContextOptions options): base(options) 
        {
        }
    }
}