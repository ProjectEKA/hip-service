namespace In.ProjectEKA.HipService.DataFlow.Database
{
    using Microsoft.EntityFrameworkCore;
    using Model;
    using Newtonsoft.Json;

    public class DataFlowContext : DbContext
    {
        public DbSet<DataFlowRequest> DataFlowRequest { get; set; }
        public DbSet<LinkData> LinkData { get; set; }

        public DataFlowContext(DbContextOptions<DataFlowContext> options) : base(options)
        {
        }

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

            modelBuilder.Entity<LinkData>().Property(e => e.Data)
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