namespace In.ProjectEKA.HipService
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IdentityModel.Tokens.Jwt;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Reflection;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Common;
    using Consent;
    using Consent.Database;
    using DataFlow;
    using DataFlow.Database;
    using DataFlow.Encryptor;
    using DefaultHip.DataFlow;
    using DefaultHip.Discovery;
    using DefaultHip.Link;
    using Discovery;
    using Discovery.Database;
    using Gateway;
    using Hangfire;
    using Hangfire.MemoryStorage;
    using HipLibrary.Matcher;
    using HipLibrary.Patient;
    using In.ProjectEKA.HipService.OpenMrs;
    using Link;
    using Link.Database;
    using MessagingQueue;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.IdentityModel.Logging;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Serilog;

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
            HttpClient = new HttpClient(clientHandler)
            {
                Timeout = TimeSpan.FromSeconds(Configuration.GetSection("Gateway:timeout").Get<int>())
            };
            IdentityModelEventSource.ShowPII = true;
        }

        private IConfiguration Configuration { get; }

        private HttpClient HttpClient { get; }

        public void ConfigureServices(IServiceCollection services)
        {
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
                .AddSingleton<ICollect>(new Collect("demoPatientCareContextDataMap.json"))
                .AddRabbit(Configuration)
                .Configure<OtpServiceConfiguration>(Configuration.GetSection("OtpService"))
                .Configure<DataFlowConfiguration>(Configuration.GetSection("dataFlow"))
                .Configure<HipConfiguration>(Configuration.GetSection("hip"))
                .AddScoped<ILinkPatientRepository, LinkPatientRepository>()
                .AddScoped<IPatientRepository, OpenMrsPatientRepository>()
                .AddSingleton<IMatchingRepository, OpenMrsPatientMatchingRepository>()
                .AddScoped<ICareContextRepository, OpenMrsCareContextRepository>()
                .AddScoped<IDiscoveryRequestRepository, DiscoveryRequestRepository>()
                .AddScoped<IPatientDiscovery, PatientDiscovery>()
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
                .AddScoped<IHealthInformationRepository, HealthInformationRepository>()
                .AddSingleton(Configuration.GetSection("Gateway").Get<GatewayConfiguration>())
                .AddSingleton(new GatewayClient(HttpClient,
                    Configuration.GetSection("Gateway").Get<GatewayConfiguration>()))
                .AddScoped<IGatewayClient, GatewayClient>()
                .AddSingleton(Configuration.GetSection("OpenMrs").Get<OpenMrsConfiguration>())
                .AddSingleton(new OpenMrsClient(HttpClient,
                    Configuration.GetSection("OpenMrs").Get<OpenMrsConfiguration>()))
                .AddScoped<IOpenMrsClient, OpenMrsClient>()
                .AddScoped<IPatientDal, FhirDiscoveryDataSource>()
                .AddScoped<IPhoneNumberRepository, OpenMrsPhoneNumberRepository>()
                .AddTransient<IDataFlow, DataFlow.DataFlow>()
                .AddRouting(options => options.LowercaseUrls = true)
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "0.0.1",
                        Title = "Health Information Provider",
                        Description =
                            "Clinical establishments which generate or store customer data in digital form."
                            + " These include hospitals, primary or secondary health care centres,"
                            + " nursing homes, diagnostic centres, clinics, medical device companies"
                            + " and other such entities as may be identified by regulatory authorities from time to time.",
                    });

                    // this is necessary to use due to we're using Newtonsoft JSON conversion until there's support for it
                    // see article: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1269
#pragma warning disable CS0618 // Type or member is obsolete
                    c.DescribeAllEnumsAsStrings();
#pragma warning restore CS0618 // Type or member is obsolete

                    // Set the comments path for the Swagger JSON and UI.
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    c.IncludeXmlComments(xmlPath);
                })
                .AddSwaggerGenNewtonsoftSupport()
                .AddControllers()
                .AddNewtonsoftJson(
                    options => { options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore; })
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
                    options.Authority = $"{Configuration.GetValue<string>("Gateway:url")}/{Constants.CURRENT_VERSION}";
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
                            if (!IsTokenValid(context))
                                context.Fail("Unable to validate token.");
                            return Task.CompletedTask;
                        }
                    };
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Use(async (context, next) =>
            {
                Stopwatch timer = new Stopwatch();
                timer.Start();
                var traceId = Guid.NewGuid();
                Log.Information($"Request {traceId} received.");

                await next.Invoke();

                timer.Stop();
                Log.Information($"Request {traceId} served in {timer.ElapsedMilliseconds}ms.");
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "HIP Service");
            });

            app.UseStaticFilesWithYaml()
                .UseRouting()
                .UseIf(!env.IsDevelopment(), x => x.UseHsts())
                .UseIf(env.IsDevelopment(), x => x.UseDeveloperExceptionPage())
                .UseSerilogRequestLogging()
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoints => { endpoints.MapControllers(); })
                .UseHangfireServer(new BackgroundJobServerOptions
                {
                    CancellationCheckInterval = TimeSpan.FromMinutes(
                        Configuration.GetSection("BackgroundJobs:cancellationCheckInterval").Get<int>())
                });

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

        private static bool CheckRoleInAccessToken(JwtSecurityToken accessToken)
        {
            if (!(accessToken.Payload["realm_access"] is JObject resourceAccess))
                return false;
            var token = new Token(resourceAccess["roles"]?.ToObject<List<string>>() ?? new List<string>());
            return token.Roles.Contains("gateway", StringComparer.OrdinalIgnoreCase);
        }

        private static bool IsTokenValid(TokenValidatedContext context)
        {
            const string claimTypeClientId = "clientId";
            var accessToken = context.SecurityToken as JwtSecurityToken;
            if (!CheckRoleInAccessToken(accessToken))
                return false;
            if (!context.Principal.HasClaim(claim => claim.Type == claimTypeClientId))
                return false;
            var clientId = context.Principal.Claims.First(claim => claim.Type == claimTypeClientId).Value;
            context.Request.Headers["X-GatewayID"] = clientId;
            return true;
        }
    }
}