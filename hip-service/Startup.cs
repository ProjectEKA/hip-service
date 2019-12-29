using hip_library.Patient;
using hip_service.Discovery.Patient;
using hip_service.Discovery.Patients;
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
                .AddSingleton<DiscoveryUseCase>()
                .AddSingleton<PatientsDiscovery>(new PatientsDiscovery(new PatientMatchingRepository("Resources/patients.json"), new DiscoveryUseCase()))
                .AddTransient<IDiscovery, PatientDiscovery>()
                .AddRouting(options => options.LowercaseUrls = true)
                .AddControllers()
                .AddNewtonsoftJson(options =>{});

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) =>
            app.UseStaticFilesWithYaml()
            .UseRouting()
            .UseIf(!env.IsDevelopment(), x => x.UseHsts())
            .UseIf(env.IsDevelopment(), x => x.UseDeveloperExceptionPage())
            .UseCustomOpenAPI()
            .UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
    }
}