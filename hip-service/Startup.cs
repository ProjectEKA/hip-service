using hip_library.Patient;
using hip_service.Discovery.Patient;
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
                .AddSingleton<IMatchingRepository>(new PatientMatchingRepository("Resources/patients.json"))
                .AddSingleton<PatientDiscovery>()
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