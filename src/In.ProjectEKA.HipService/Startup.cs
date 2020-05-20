using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.MemoryStorage;
using In.ProjectEKA.DefaultHip.DataFlow;
using In.ProjectEKA.DefaultHip.Discovery;
using In.ProjectEKA.DefaultHip.Link;
using In.ProjectEKA.HipLibrary.Matcher;
using In.ProjectEKA.HipLibrary.Patient;
using In.ProjectEKA.HipService.Common;
using In.ProjectEKA.HipService.Consent;
using In.ProjectEKA.HipService.Consent.Database;
using In.ProjectEKA.HipService.DataFlow;
using In.ProjectEKA.HipService.DataFlow.Database;
using In.ProjectEKA.HipService.DataFlow.Encryptor;
using In.ProjectEKA.HipService.Discovery;
using In.ProjectEKA.HipService.Discovery.Database;
using In.ProjectEKA.HipService.Link;
using In.ProjectEKA.HipService.Link.Database;
using In.ProjectEKA.HipService.MessagingQueue;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;

namespace In.ProjectEKA.HipService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            var clientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (
                    sender,
                    cert,
                    chain,
                    sslPolicyErrors) => true
            };
            HttpClient = new HttpClient(clientHandler);
        }

        private IConfiguration Configuration { get; }

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
                .AddHangfire(config => { config.UseMemoryStorage(); })
                .AddSingleton<IEncryptor, Encryptor>()
                .AddSingleton<IPatientRepository>(new PatientRepository("demoPatients.json"))
                .AddSingleton<ICollect>(new Collect("demoPatientCareContextDataMap.json"))
                .AddSingleton<IPatientRepository>(new PatientRepository("demoPatients.json"))
                .AddRabbit(Configuration)
                .Configure<OtpServiceConfiguration>(Configuration.GetSection("OtpService"))
                .Configure<DataFlowConfiguration>(Configuration.GetSection("dataFlow"))
                .Configure<HipConfiguration>(Configuration.GetSection("hip"))
                .AddScoped<ILinkPatientRepository, LinkPatientRepository>()
                .AddSingleton<IMatchingRepository>(new PatientMatchingRepository("demoPatients.json"))
                .AddScoped<IDiscoveryRequestRepository, DiscoveryRequestRepository>()
                .AddScoped<PatientDiscovery>()
                .AddScoped<LinkPatient>()
                .AddScoped<ReferenceNumberGenerator>()
                .AddSingleton(Configuration)
                .AddSingleton<DataFlowClient>()
                .AddSingleton<DataFlowNotificationClient>()
                .AddSingleton<DataEntryFactory>()
                .AddSingleton<DataFlowMessageHandler>()
                .AddSingleton(HttpClient)
                .AddScoped<IPatientVerification, PatientVerification>()
                .AddScoped<IConsentRepository, ConsentRepository>()
                .AddHostedService<MessagingQueueListener>()
                .AddScoped<IDataFlowRepository, DataFlowRepository>()
                .AddSingleton(Configuration.GetSection("authServer").Get<CentralRegistryConfiguration>())
                .AddScoped<IHealthInformationRepository, HealthInformationRepository>()
                .AddSingleton(new CentralRegistryClient(HttpClient,
                    Configuration.GetSection("authServer").Get<CentralRegistryConfiguration>()))
                .AddSingleton(new GatewayClient(HttpClient, new CentralRegistryClient(
                        HttpClient,
                        Configuration.GetSection("authServer").Get<CentralRegistryConfiguration>()),
                    Configuration.GetSection("Gateway").Get<GatewayConfiguration>()))
                .AddTransient<IDataFlow, DataFlow.DataFlow>()
                .AddRouting(options => options.LowercaseUrls = true)
                .AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                })
                .Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    // Need to validate Audience and Issuer properly
                    options.Authority = Configuration.GetValue<string>("authServer:url");
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        AudienceValidator = (audiences, token, parameters) => true,
                        IssuerValidator = (issuer, token, parameters) => token.Issuer
                    };
                    options.RequireHttpsMetadata = false;
                    options.IncludeErrorDetails = true;
                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            const string claimTypeClientId = "clientId";
                            if (!context.Principal.HasClaim(claim => claim.Type == claimTypeClientId))
                            {
                                context.Fail($"Claim {claimTypeClientId} is not present in the token.");
                            }
                            else
                            {
                                context.Request.Headers["X-ConsentManagerID"] =
                                    context.Principal.Claims.First(claim => claim.Type == claimTypeClientId).Value;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

        private HttpClient HttpClient { get; }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFilesWithYaml()
                .UseRouting()
                .UseIf(!env.IsDevelopment(), x => x.UseHsts())
                .UseIf(env.IsDevelopment(), x => x.UseDeveloperExceptionPage())
                .UseCustomOpenApi()
                .UseSerilogRequestLogging()
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoints => { endpoints.MapControllers(); })
                .UseHangfireServer();

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