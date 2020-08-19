namespace In.ProjectEKA.HipService.Consent.Database
{
    using System;
    using Common.Model;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Model;
    using Newtonsoft.Json;

    public class ConsentContext : DbContext
    {
        public ConsentContext(DbContextOptions<ConsentContext> options) : base(options)
        {
        }

        public DbSet<Consent> ConsentArtefact { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Consent>().HasIndex(p => p.ConsentArtefactId).IsUnique();
            modelBuilder.Entity<Consent>().Property(p => p.Status)
                .HasConversion(status => status.ToString(),
                    x => (ConsentStatus) Enum.Parse(typeof(ConsentStatus), x));
            modelBuilder.ApplyConfiguration(new ConsentArtefactConfiguration());
        }
    }

    public class ConsentArtefactConfiguration : IEntityTypeConfiguration<Consent>
    {
        public void Configure(EntityTypeBuilder<Consent> builder)
        {
            builder.Property(e => e.ConsentArtefact).HasConversion(
                v => JsonConvert.SerializeObject(v,
                    new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore}),
                v => JsonConvert.DeserializeObject<ConsentArtefact>(v,
                    new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore}));
        }
    }
}