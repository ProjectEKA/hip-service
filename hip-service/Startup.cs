using hip_library.Patient;
using hip_service.Discovery.Patient;
using hip_service.Discovery.Patients;
using hip_service.Link.Patient;
using hip_service.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace hip_service
{
    public class Startup
    {

        public void ConfigureServices(IServiceCollection services) =>
            services
                .AddSingleton<IPatientRepository>(new PatientRepository("Resources/patients.json"))
                .AddSingleton<ILinkPatientRepository>(new LinkPatientRepository("Resources/patients.json"))
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