namespace In.ProjectEKA.OtpService
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Serilog;
    using System;
    using Microsoft.Extensions.DependencyInjection;
    public static class Program
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
                .ReadFrom.Configuration(host.Services.GetRequiredService<IConfiguration>())
                .CreateLogger();

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseStartup<Startup>();
    }
}