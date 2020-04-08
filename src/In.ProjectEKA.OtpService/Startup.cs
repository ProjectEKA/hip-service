namespace In.ProjectEKA.OtpService
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using Clients;
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
                .AddSingleton<ISmsClient, SmsClient>()
                .AddSingleton(new OtpProperties(Configuration.GetValue<int>("expiryInMinutes")))
                .AddScoped<IOtpRepository, OtpRepository>()
                .AddScoped<IOtpGenerator, OtpGenerator>()
                .AddScoped<INotificationService, NotificationService>()
                .AddScoped<OtpVerifier, OtpVerifier>()
                .AddScoped<OtpSender, OtpSender>()
                .AddScoped<FakeOtpSender, FakeOtpSender>()
                .AddScoped(serviceProvider =>
                    new OtpSenderFactory(serviceProvider.GetService<OtpSender>(),
                        serviceProvider.GetService<FakeOtpSender>(),
                        Configuration.GetValue<string>("whitelisted:numbers")?.Split(",").ToList()))
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