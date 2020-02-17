namespace In.ProjectEKA.HipService
{
    using System.Net.Http;
    using System.Text.Json;
    using In.ProjectEKA.DefaultHip.DataFlow;
    using In.ProjectEKA.DefaultHip.Discovery;
    using In.ProjectEKA.DefaultHip.Link;
    using In.ProjectEKA.HipLibrary.Patient;
    using In.ProjectEKA.HipService.Consent;
    using In.ProjectEKA.HipService.Consent.Database;
    using In.ProjectEKA.HipService.DataFlow;
    using In.ProjectEKA.HipService.DataFlow.Database;
    using In.ProjectEKA.HipService.Discovery;
    using In.ProjectEKA.HipService.Discovery.Database;
    using In.ProjectEKA.HipService.Link;
    using In.ProjectEKA.HipService.Link.Database;
    using In.ProjectEKA.HipService.MessagingQueue;
    using In.ProjectEKA.HipService.Middleware;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Newtonsoft.Json;
    using Serilog;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            var clientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };
            HttpClient = new HttpClient(clientHandler);
        }

        private IConfiguration Configuration { get; }
        private HttpClient HttpClient { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddDbContext<LinkPatientContext>(options =>
                    options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"),
                        x => x.MigrationsAssembly("In.ProjectEKA.HipService")))
                .AddDbContext<DiscoveryContext>(options =>
                    options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"),
                        x => x.MigrationsAssembly("In.ProjectEKA.HipService")))
                .AddDbContext<DataFlowContext>(options =>
                    options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"),
                        x => x.MigrationsAssembly("In.ProjectEKA.HipService")))
                .AddDbContext<ConsentContext>(options =>
                    options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"),
                        x => x.MigrationsAssembly("In.ProjectEKA.HipService")))
                .AddSingleton<IPatientRepository>(new PatientRepository("Resources/patients.json"))
                .AddSingleton<ICollect>(new Collect("../In.ProjectEKA.DefaultHip/Resources/observation.json"))
                .AddRabbit(Configuration)
                .Configure<OtpServiceConfiguration>(Configuration.GetSection("OtpService"))
                .AddScoped<ILinkPatientRepository, LinkPatientRepository>()
                .AddSingleton<IMatchingRepository>(new PatientMatchingRepository("Resources/patients.json"))
                .AddScoped<IDiscoveryRequestRepository, DiscoveryRequestRepository>()
                .AddScoped<PatientDiscovery>()
                .AddTransient<IDiscovery, PatientDiscovery>()
                .AddScoped<IReferenceNumberGenerator, ReferenceNumberGenerator>()
                .AddTransient<ILink, LinkPatient>()
                .AddSingleton(Configuration)
                .AddSingleton<DataFlowClient>()
                .AddSingleton(HttpClient)
                .AddScoped<IPatientVerification, PatientVerification>()
                .AddScoped<IConsentRepository, ConsentRepository>()
                .AddHostedService<MessagingQueueListener>()
                .AddScoped<IDataFlowRepository, DataFlowRepository>()
                .AddTransient<IDataFlow, DataFlow.DataFlow>()
                .AddRouting(options => options.LowercaseUrls = true)
                .AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFilesWithYaml()
                .UseRouting()
                .UseIf(!env.IsDevelopment(), x => x.UseHsts())
                .UseIf(env.IsDevelopment(), x => x.UseDeveloperExceptionPage())
                .UseCustomOpenAPI()
                .UseSerilogRequestLogging()
                .UseConsentManagerIdentifierMiddleware()
                .UseEndpoints(endpoints => { endpoints.MapControllers(); });

            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var linkContext = serviceScope.ServiceProvider.GetService<LinkPatientContext>();
            linkContext.Database.Migrate();
            var discoveryContext = serviceScope.ServiceProvider.GetService<DiscoveryContext>();
            discoveryContext.Database.Migrate();
            var dataFlowContext = serviceScope.ServiceProvider.GetService<DataFlowContext>();
            dataFlowContext.Database.Migrate();
            var consentContext = serviceScope.ServiceProvider.GetService<ConsentContext>();
            consentContext.Database.Migrate();
        }
    }
}