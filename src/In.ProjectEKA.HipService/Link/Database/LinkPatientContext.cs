namespace In.ProjectEKA.HipService.Link.Database
{
    using System.Collections.Generic;
    using DataFlow;
    using Microsoft.EntityFrameworkCore;
    using Model;
    using Newtonsoft.Json;

    public class LinkPatientContext : DbContext
    {
        public LinkPatientContext(DbContextOptions<LinkPatientContext> options) : base(options)
        {
        }

        public DbSet<LinkEnquires> LinkEnquires { get; set; }
        
        public DbSet<LinkedAccounts> LinkedAccounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CareContext>()
                .HasKey("CareContextNumber", "LinkReferenceNumber")
                .HasName("Id");

            modelBuilder.Entity<LinkEnquires>()
                .HasMany(l => l.CareContexts)
                .WithOne(c => c.LinkEnquires)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<LinkedAccounts>(builder =>
            {
                builder.Property(p => p.DateTimeStamp)
                    .HasDefaultValueSql("now()");
            });
            
            modelBuilder.Entity<LinkedAccounts>()
                .Property(e => e.CareContexts)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v,
                        new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore}),
                    v => JsonConvert.DeserializeObject<List<string>>(v,
                        new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore}));
        }
    }
}