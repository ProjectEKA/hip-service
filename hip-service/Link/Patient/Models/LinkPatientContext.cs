using hip_service.OTP.Models;
using Microsoft.EntityFrameworkCore;

namespace hip_service.Link.Patient.Models
{
    public class LinkPatientContext: DbContext
    {
        public DbSet<LinkRequest> LinkRequest { get; set; }
        public DbSet<OtpRequest> OtpRequests { get; set; }
        public DbSet<LinkedCareContext> LinkedCareContexts { get; set; }

        public LinkPatientContext(DbContextOptions<LinkPatientContext> options): base(options) 
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LinkedCareContext>()
                .HasKey( "CareContextNumber","LinkReferenceNumber")
                .HasName("Id");
            
            modelBuilder.Entity<LinkRequest>()
                .HasMany(l => l.CareContexts)
                .WithOne(c => c.LinkRequest)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}