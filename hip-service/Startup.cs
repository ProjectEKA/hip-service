using hip_library.Patient;
using hip_service.Discovery.Patient;
using hip_service.Discovery.Patients;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace hip_service
{
    public class Startup
    {
        private readonly IConfiguration Configuration;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services) =>
            services
                .AddSingleton<IPatientRepository>(new PatientRepository("Resources/patients.json"))
                .AddSingleton<DiscoveryUseCase>()
                .AddTransient<IDiscovery, PatientDiscovery>()
                .AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) =>
            app.UseStaticFilesWithYaml()
            .UseRouting()
            .UseIf(!env.IsDevelopment(), x => x.UseHsts())
            .UseIf(env.IsDevelopment(), x => x.UseDeveloperExceptionPage())
            .UseCustomOpenAPI();
    }
}