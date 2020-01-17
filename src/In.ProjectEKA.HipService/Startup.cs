using System.Net.Http;

namespace In.ProjectEKA.HipService
{
    using System.Text.Json;
    using Discovery.Patient;
    using HipLibrary.Patient;
    using Link.Patient;
    using Link.Patient.Model;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Middleware;
    using OTP;

    public class Startup
    {
        public IConfiguration Configuration { get; }
        public HttpClient HttpClient { get; }

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
                    options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")))
                .AddSingleton<IPatientRepository>(new PatientRepository("Resources/patients.json"))
                .AddScoped<ILinkPatientRepository, LinkPatientRepository>()
                .AddSingleton<IMatchingRepository>(new PatientMatchingRepository("Resources/patients.json"))
                .AddSingleton<PatientDiscovery>()
                .AddTransient<IDiscovery, PatientDiscovery>()
                .AddScoped<IReferenceNumberGenerator, ReferenceNumberGenerator>()
                .AddTransient<ILink, LinkPatient>()
                .AddScoped<IOtpRepository, OtpRepository>()
                .AddScoped<OtpVerification>()
                .AddSingleton(Configuration)
                .AddSingleton(HttpClient)
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
        }
    }
}