using Bogus;
using In.ProjectEKA.DefaultHip.Link;
using Microsoft.Extensions.Configuration;

namespace In.ProjectEKA.DefaultHipTest.Link.Builder
{
    public class TestBuilder
    {
        private static Faker faker;
        internal static Faker Faker() => faker ??= new Faker();
        internal static Faker<OtpMessage> otpMessage()
        {
            return new Faker<OtpMessage>();
        }
        
        public static IConfigurationRoot GetIConfigurationRoot()
        {            
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}