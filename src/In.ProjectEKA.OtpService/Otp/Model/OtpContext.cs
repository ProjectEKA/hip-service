namespace In.ProjectEKA.OtpService.Otp.Model
{
    using Microsoft.EntityFrameworkCore;

    public class OtpContext : DbContext
    {
        public DbSet<OtpRequest> OtpRequests { get; set; }

        public OtpContext(DbContextOptions options): base(options) 
        {
        }
    }
}