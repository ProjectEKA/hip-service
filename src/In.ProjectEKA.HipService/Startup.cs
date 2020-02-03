
namespace In.ProjectEKA.HipService
{
    using System.Text.Json;
    using DefaultHip;
    using HipLibrary.Patient;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Middleware;
    using System.Net.Http;
    using DefaultHip.Discovery;
    using Discovery;
    using Discovery.Database;
    using In.ProjectEKA.DefaultHip.Link;
    using In.ProjectEKA.DefaultHip.Link.Database;
    using Link;
    using Link.Database;

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
                        x => x.MigrationsAssembly("In.ProjectEKA.DefaultHip")))
                .AddDbContext<DiscoveryContext>(options =>
                    options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"),
                        x => x.MigrationsAssembly("In.ProjectEKA.DefaultHip")))
                .AddSingleton<IPatientRepository>(new PatientRepository("Resources/patients.json"))
                .AddScoped<ILinkPatientRepository, LinkPatientRepository>()
                .AddSingleton<IMatchingRepository>(new PatientMatchingRepository("Resources/patients.json"))
                .AddScoped<Discovery.IDiscoveryRequestRepository, DiscoveryRequestRepository>()
                .AddScoped<PatientDiscovery>()
                .AddTransient<IDiscovery, PatientDiscovery>()
                .AddScoped<IReferenceNumberGenerator, ReferenceNumberGenerator>()
                .AddTransient<ILink, LinkPatient>()
                .AddSingleton(Configuration)
                .AddSingleton(HttpClient)
                .Configure<OtpServiceConfiguration>(Configuration.GetSection("OtpService"))
                .AddScoped<IPatientVerification, PatientVerification>()
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
                .UseConsentManagerIdentifierMiddleware()
                .UseEndpoints(endpoints => { endpoints.MapControllers(); });

            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var linkContext = serviceScope.ServiceProvider.GetService<LinkPatientContext>();
            linkContext.Database.Migrate();
            var discoveryContext = serviceScope.ServiceProvider.GetService<DiscoveryContext>();
            discoveryContext.Database.Migrate();
        }
    }
}