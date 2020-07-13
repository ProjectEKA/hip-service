using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace In.ProjectEKA.HipServiceTest.OpenMrs
{
    [Collection("Gateway Client Tests")]
    public class OpenMrsClientTest
    {
        [Fact]
        public async Task ShouldGetPatientDataRealCallAsync()
        {
        //Given
        // Disable SSL verification in text only
        var handler = new HttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        var httpClient = new HttpClient(handler);
        var openmrsConfiguration = new OpenMrsConfiguration {
            Url = "https://192.168.33.10/openmrs/",
            Username = "superman",
            Password = "Admin123"
            };
        var openmrsClient = new OpenMrsClient(httpClient, openmrsConfiguration);
        //When
        var response = await openmrsClient.GetAsync("ws/fhir2/Patient");
        //Then
        response.Should().NotBeNull();
        response.Should().NotBe(String.Empty);
        Console.WriteLine(response);
        }
    }
}