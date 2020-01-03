using System;
using hip_library.Patient;
using hip_service.Discovery.Patient;
using hip_service.Discovery.Patients;
using hip_service.Link.Patient;
using hip_service.Link.Patient.Models;
using hip_service.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace hip_service
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services) =>
            
            services
                .AddDbContext<LinkPatientContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")))
                .AddSingleton<IPatientRepository>(new Discovery.Patient.PatientRepository("Resources/patients.json"))
                .AddSingleton(new Link.Patient.PatientRepository("Resources/patients.json"))
                .AddScoped<ILinkPatientRepository, LinkPatientRepository>()
                .AddSingleton<DiscoveryUseCase>()
                .AddTransient<IDiscovery, PatientDiscovery>()
                .AddTransient<ILink, LinkPatient>()
                .AddRouting(options => options.LowercaseUrls = true)
                .AddControllers()
                .AddNewtonsoftJson(options =>{});

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) =>
            
            app.UseStaticFilesWithYaml()
                .UseRouting()
                .UseIf(!env.IsDevelopment(), x => x.UseHsts())
                .UseIf(env.IsDevelopment(), x => x.UseDeveloperExceptionPage())
                .UseCustomOpenAPI()
                .UseSecurityHeadersMiddleware()
                .UseEndpoints(endpoints => { endpoints.MapControllers(); });
        
        
    }
}