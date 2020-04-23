namespace In.ProjectEKA.HipService
{
    using System;
    using System.Reflection;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Serilog;
    using Serilog.Exceptions;
    using Serilog.Sinks.Elasticsearch;

    public class Program
    {
        public static void Main(string[] args) =>
            LogAndRunAsync(CreateWebHostBuilder(args).Build());

        public static void LogAndRunAsync(IWebHost host)
        {
            if (host is null)
            {
                throw new ArgumentNullException(nameof(host));
            }
            
            Log.Logger = CreateLogger(host);
            try
            {
                Log.Information("Started application");
                host.Run();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception exception)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                Log.Fatal(exception, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
        private static ILogger CreateLogger(IWebHost host) =>
            new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .Enrich.WithMachineName()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(ConfigureElasticSink(
                    new ConfigurationBuilder()
                                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                                        true)
                                    .Build(),
                    Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")))
                .Enrich.WithProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
                .ReadFrom.Configuration(host.Services.GetRequiredService<IConfiguration>())
                .CreateLogger();
                
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseStartup<Startup>();
        }
        
        private static ElasticsearchSinkOptions ConfigureElasticSink(IConfigurationRoot configuration, string environment)
        {
            return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
            };
        }
    }
}
