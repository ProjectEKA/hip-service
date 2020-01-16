using hip_service.Discovery.Patient.Model;
using Microsoft.EntityFrameworkCore;

namespace hip_service.Discovery.Patient.Database
{
    public class DiscoveryContext : DbContext
    {
        public DbSet<DiscoveryRequest> DiscoveryRequest { get; set; }

        public DiscoveryContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DiscoveryRequest>(builder =>
            {
                builder.Property(p => p.Timestamp)
                    .HasDefaultValueSql("now()");
            });
        }
    }
}