namespace In.ProjectEKA.HipService.DataFlow.Database
{
    using Model;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Newtonsoft.Json;
    public class DataFlowContext : DbContext
    {
        public DbSet<DataFlowRequest> DataFlowRequest { get; set; }
        
        public DataFlowContext(DbContextOptions<DataFlowContext> options) : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new DataFlowRequestConfiguration());
            modelBuilder.Entity<DataFlowRequest>()
                .HasKey(d => new { d.TransactionId});
        }
    }
    
    public class DataFlowRequestConfiguration : IEntityTypeConfiguration<DataFlowRequest>
    {
        public void Configure(EntityTypeBuilder<DataFlowRequest> builder)
        {
            builder.Property(e => e.HealthInformationRequest).HasConversion(
                v => JsonConvert.SerializeObject(v,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                v => JsonConvert.DeserializeObject<HealthInformationRequest>(v,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }
    }
}