namespace In.ProjectEKA.HipService.DataFlow.Database
{
    using Microsoft.EntityFrameworkCore;
    using Model;
    using Newtonsoft.Json;

    public class DataFlowContext : DbContext
    {
        public DataFlowContext(DbContextOptions<DataFlowContext> options) : base(options)
        {
        }

        public DbSet<DataFlowRequest> DataFlowRequest { get; set; }
        public DbSet<HealthInformation> HealthInformation { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<DataFlowRequest>()
                .HasKey(d => new {d.TransactionId});
            modelBuilder.Entity<DataFlowRequest>()
                .Property(e => e.HealthInformationRequest)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v,
                        new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore}),
                    v => JsonConvert.DeserializeObject<HealthInformationRequest>(v,
                        new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore}));

            modelBuilder.Entity<HealthInformation>()
                .Property(e => e.Data)
                .HasConversion(
                    v =>
                        JsonConvert.SerializeObject(v,
                            new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore}),
                    v =>
                        JsonConvert.DeserializeObject<Entry>(v,
                            new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore}));
        }
    }
}