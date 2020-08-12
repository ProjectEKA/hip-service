namespace In.ProjectEKA.HipServiceTest.Common.TestServer
{
    using System.Net.Http;
    using System.Text.Json;
    using Hangfire;
    using In.ProjectEKA.HipLibrary.Matcher;
    using In.ProjectEKA.HipLibrary.Patient;
    using In.ProjectEKA.HipService.Discovery;
    using In.ProjectEKA.HipService.Gateway;
    using In.ProjectEKA.HipService.Link;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using Newtonsoft.Json;

    public class TestStartup
    {
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoints => { endpoints.MapControllers(); })
                .UseMiddleware<AuthenticatedTestRequestMiddleware>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            Mock<IDiscoveryRequestRepository> discoveryRequestRepository = new Mock<IDiscoveryRequestRepository>();
            Mock<ILinkPatientRepository> linkPatientRepository = new Mock<ILinkPatientRepository>();
            Mock<IMatchingRepository> matchingRepository = new Mock<IMatchingRepository>();
            Mock<IPatientRepository> patientRepository = new Mock<IPatientRepository>();
            Mock<ICareContextRepository> careContextRepository = new Mock<ICareContextRepository>();
            Mock<IBackgroundJobClient> backgroundJobClient = new Mock<IBackgroundJobClient>();

            var patientDiscovery = new PatientDiscovery(
                matchingRepository.Object,
                discoveryRequestRepository.Object,
                linkPatientRepository.Object,
                patientRepository.Object,
                careContextRepository.Object);

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(handlerMock.Object);
            var gatewayConfiguration = new GatewayConfiguration { Url = "http://someUrl" };
            var gatewayClient = new GatewayClient(httpClient, gatewayConfiguration);

            services
                .AddScoped<IPatientDiscovery, PatientDiscovery>(provider => patientDiscovery)
                .AddScoped<IGatewayClient, GatewayClient>(provider => gatewayClient)
                .AddScoped(provider => backgroundJobClient.Object)
                .AddRouting(options => options.LowercaseUrls = true)
                .AddControllers()
                .AddNewtonsoftJson(
                    options => { options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore; })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                })
                .Services
                    .AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
        }
    }
}
