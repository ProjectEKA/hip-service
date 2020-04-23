namespace In.ProjectEKA.HipService.Discovery.Database
{
    using Microsoft.EntityFrameworkCore;
    using Model;

    public class DiscoveryContext : DbContext
    {
        public DbSet<DiscoveryRequest> DiscoveryRequest { get; set; }

        public DiscoveryContext(DbContextOptions<DiscoveryContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DiscoveryRequest>(builder =>
            {
                builder.Property(p => p.Timestamp)
                    .HasDefaultValueSql("now()");
                builder
                    .HasIndex(p => p.RequestId)
                    .IsUnique();
            });
        }
    }
}