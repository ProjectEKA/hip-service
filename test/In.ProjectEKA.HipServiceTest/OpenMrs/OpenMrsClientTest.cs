using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Moq;
using Moq.Protected;
using Xunit;

namespace In.ProjectEKA.HipServiceTest.OpenMrs
{
    [Collection("Gateway Client Tests")]
    public class OpenMrsClientTest
    {
        [Fact(Skip="it is a real call and needs local setup")]
        [Trait("Category", "Infrastructure")]
        public async System.Threading.Tasks.Task ShouldGetPatientDataRealCallAsync()
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
            response.StatusCode.Should().Be(200);
        }

        [Fact]
        public async System.Threading.Tasks.Task ShouldPropagateStatusWhenCredentialsFailed()
        {
            //Given
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(handlerMock.Object);
            var openmrsConfiguration = new OpenMrsConfiguration
            {
                Url = "https://someurl/openmrs/",
                Username = "someusername",
                Password = "somepassword"
            };
            var openmrsClient = new OpenMrsClient(httpClient, openmrsConfiguration);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                })
                .Verifiable();

            //When
            var response = await openmrsClient.GetAsync("path/to/resource");
            //Then
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async System.Threading.Tasks.Task ShouldInterrogateTheRightDataSource()
        {
            //Given
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Loose);
            var httpClient = new HttpClient(handlerMock.Object);
            var openmrsConfiguration = new OpenMrsConfiguration
            {
                Url = "https://someurl/openmrs/",
                Username = "someusername",
                Password = "somepassword"
            };
            var openmrsClient = new OpenMrsClient(httpClient, openmrsConfiguration);

            var wasCalledWithTheRightUri = false;
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((response, token) =>
                {
                    if (response.RequestUri.AbsoluteUri == "https://someurl/openmrs/path/to/resource")
                    {
                        wasCalledWithTheRightUri = true;
                    }
                })
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                })
                .Verifiable();

            //When
            await openmrsClient.GetAsync("path/to/resource");
            //Then
            wasCalledWithTheRightUri.Should().BeTrue();
        }

        [Fact(Skip = "IncompleteReturnedMessage")]
        public async System.Threading.Tasks.Task ShouldReturnListOfPatientDto()
        {
            //Given
            var handlerMock = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(handlerMock.Object);
            var openmrsConfiguration = new OpenMrsConfiguration
            {
                Url = "https://someurl/openmrs/",
                Username = "someusername",
                Password = "somepassword"
            };
            var openmrsClient = new OpenMrsClient(httpClient, openmrsConfiguration);

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(@"
                    {
                        ""lastUpdated"": ""2020 - 07 - 13T07: 35:11.171 + 00:00"",
                        ""type"":""searchset"",
                        ""total"":1,
                        ""link"":[{
                            ""relation"":""self"",
                            ""url"":""http://192.168.33.10/openmrs/ws/fhir2/Patient""
                        }],
                        ""entry"":[{
                            ""resource"":{
                                ""resourceType"":""Patient"",
                                ""id"":""f22d0221-d927-40da-a72c-b8145dcc4b50"",
                                ""identifier"":[{
                                    ""id"":""bdfaf98a-9e23-4cf5-889a-4346c3586458"",
                                    ""use"":""official"",
                                    ""system"":""Patient Identifier"",
                                    ""value"":""GAN203006""
                                },{
                                    ""id"":""cb8e66de-b428-42f4-b7da-5da0eddf95c4"",
                                    ""use"":""usual"",
                                    ""system"":""National ID"",
                                    ""value"":""NAT2804""
                                }],
                                ""active"":true,
                                ""name"":[{
                                    ""id"":""9544d7ed-327d-4800-9a26-2a9c60c740fa"",
                                    ""family"":""Test"",
                                    ""given"":[""Gwan""]
                                }],
                                ""gender"":""male"",
                                ""birthDate"":""2005-02-07"",
                                ""deceasedBoolean"":false,
                                ""address"":[{
                                    ""id"":""5c1b2f63-3c74-48e2-b76e-fcb9121049ea"",
                                    ""use"":""home"",
                                    ""city"":""Betong""
                                }]
                            }
                        }]
                    }")
                        })
                .Verifiable();

            //When
            var response = await openmrsClient.GetAsync("path/to/resource");
            //Then
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);

            var parser = new FhirJsonParser();

            var patient = parser.Parse<Patient>(content);

            patient.Should().Be(new Patient
            {
                Name = new List<HumanName>{ new HumanName
                    {
                        Family = "Test",
                        Given = new List<string>{"Gwan"}
                    }
                },
                BirthDate = "2005-02-07",
                Gender = AdministrativeGender.Male
            });

        }
    }

    internal class PatientDto
    {
        public string Id { get; set; }
        public NameDto Name { get; set; }
        public string Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public AddressDto Address { get; set; }
    }

    public class AddressDto
    {
        public string Id { get; set; }
        public string Use { get; set; }
        public string City { get; set; }
    }

    public class NameDto
    {
         public string Id { get; set; }
         public string Family { get; set; }
         public string Given { get; set; }
    }
}