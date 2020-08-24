namespace In.ProjectEKA.HipService.Discovery.Database
{
    using Microsoft.EntityFrameworkCore;
    using Model;

    public class DiscoveryContext : DbContext
    {
        public DiscoveryContext(DbContextOptions<DiscoveryContext> options) : base(options)
        {
        }

        public DbSet<DiscoveryRequest> DiscoveryRequest { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DiscoveryRequest>(builder =>
            {
                builder.Property(p => p.Timestamp)
                    .HasDefaultValueSql("now()");
                builder
                    .HasIndex(p => p.TransactionId)
                    .IsUnique();
            });
        }
    }
}