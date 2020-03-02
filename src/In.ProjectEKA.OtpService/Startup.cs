namespace In.ProjectEKA.OtpService
{
    using System.Text.Json;
    using Otp;
    using Otp.Model;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Notification;
    using Serilog;
    
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) =>
            services
                .AddDbContext<OtpContext>(options =>
                    options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")))
                .AddScoped<IOtpRepository, OtpRepository>()
                .AddScoped<IOtpService, OtpService>()
                .AddScoped<IOtpGenerator, OtpGenerator>()
                .AddScoped<IOtpWebHandler, OtpWebHandler>()
                .AddScoped<INotificationWebHandler, NotificationWebHandler>()
                .AddScoped<INotificationService, NotificationService>()
                .AddControllers()
                .AddNewtonsoftJson(options => { })
                .AddJsonOptions(options =>
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting()
                .UseAuthorization()
                .UseStaticFilesWithYaml()
                .UseCustomOpenAPI()
                .UseIf(!env.IsDevelopment(), x => x.UseHsts())
                .UseIf(env.IsDevelopment(), x => x.UseDeveloperExceptionPage())
                .UseSerilogRequestLogging()
                .UseEndpoints(endpoints => { endpoints.MapControllers(); });
                    
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var otpContext = serviceScope.ServiceProvider.GetService<OtpContext>();
            otpContext.Database.Migrate();
        }
            
    }
}