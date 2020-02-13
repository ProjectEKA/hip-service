namespace In.ProjectEKA.HipService
{
    using System.Net.Http;
    using System.Text.Json;
    using Consent;
    using Consent.Database;
    using DefaultHip.Discovery;
    using DefaultHip.Link;
    using Discovery;
    using Discovery.Database;
    using HipLibrary.Patient;
    using Link;
    using Link.Database;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Middleware;
    using DataFlow;
    using DataFlow.Database;
    using Serilog;
    using MessagingQueue;

    public class Startup
    {
        private IConfiguration Configuration { get; }
        private HttpClient HttpClient { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            var clientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };
            HttpClient = new HttpClient(clientHandler);
        }

        public void ConfigureServices(IServiceCollection services) =>
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
                .AddSingleton(HttpClient)
                .AddScoped<IPatientVerification, PatientVerification>()
                .AddScoped<ConsentRepository>()
                .AddHostedService<MessagingQueueListener>()
                .AddScoped<IDataFlowRepository, DataFlowRepository>()
                .AddScoped<IConsentArtefactRepository, ConsentArtefactRepository>()
                .AddTransient<IDataFlow, DataFlow.DataFlow>()
                .AddRouting(options => options.LowercaseUrls = true)
                .AddControllers()
                .AddNewtonsoftJson(options =>{})
                .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

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