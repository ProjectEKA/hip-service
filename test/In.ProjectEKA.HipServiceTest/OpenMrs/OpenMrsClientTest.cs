using System.Net.Http;
using FluentAssertions;
using Xunit;

namespace In.ProjectEKA.HipServiceTest.OpenMrs
{
    [Collection("Gateway Client Tests")]
    public class OpenMrsClientTest
    {
        [Fact]
        public void ShouldGetPatientDataRealCall()
        {
        //Given
        var httpClient = new HttpClient();
        var openmrsConfiguration = new OpenMrsConfiguration {
            Url = "https://192.168.33.10/openmrs/",
            Username = "superman",
            Password = "Admin123"
            };
        var openmrsClient = new OpenMrsClient(httpClient, openmrsConfiguration);
        //When
        var response = openmrsClient.Get("/ws/fhir2/Patient");
        //Then
        response.Should().NotBeNull();
        }
    }
}