using System;
using System.Collections.Generic;
using System.Linq;
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

        [Fact]
        [Trait("Category", "praser")]
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
                    Content = new StringContent(@"{
    ""resourceType"": ""Bundle"",
    ""id"": ""fbfee329-6108-4a7e-87b4-e047ae013c3a"",
    ""meta"": {
        ""lastUpdated"": ""2020-07-15T12:05:34.177+05:30""
    },
    ""type"": ""searchset"",
    ""total"": 8,
    ""link"": [
        {
            ""relation"": ""self"",
            ""url"": ""http://bahmni-0.92.bahmni-covid19.in/openmrs/ws/fhir2/Patient""
        }
    ],
    ""entry"": [
        {
            ""resource"": {
                ""resourceType"": ""Patient"",
                ""id"": ""1dfff08c-141b-46df-b6a2-6b69080a5000"",
                ""identifier"": [
                    {
                        ""id"": ""b760a6a5-dcdd-4529-86f9-c4f91c507b16"",
                        ""use"": ""official"",
                        ""system"": ""Patient Identifier"",
                        ""value"": ""GAN203006""
                    },
                    {
                        ""id"": ""bb9a7f86-37ea-481a-8f05-be9f32d89bab"",
                        ""use"": ""usual"",
                        ""system"": ""National ID"",
                        ""value"": ""NAT2804""
                    }
                ],
                ""active"": true,
                ""name"": [
                    {
                        ""id"": ""71b58b75-3cde-4430-996a-8e6f2d117971"",
                        ""family"": ""Hyperthyroidism"",
                        ""given"": [
                            ""Test""
                        ]
                    }
                ],
                ""gender"": ""male"",
                ""birthDate"": ""1982-05-05"",
                ""deceasedBoolean"": false,
                ""address"": [
                    {
                        ""id"": ""6a48a417-8f7b-4f8e-96f8-6c2cb02b4393"",
                        ""extension"": [
                            {
                                ""url"": ""https://fhir.openmrs.org/ext/address"",
                                ""extension"": [
                                    {
                                        ""url"": ""https://fhir.openmrs.org/ext/address#address3"",
                                        ""valueString"": ""Masturi""
                                    }
                                ]
                            }
                        ],
                        ""use"": ""home"",
                        ""city"": ""AAGDIH"",
                        ""state"": ""Chattisgarh""
                    }
                ]
            }
        },
        {
            ""resource"": {
                ""resourceType"": ""Patient"",
                ""id"": ""0b573f9a-d75d-47fe-a655-043dc2f6b4fa"",
                ""identifier"": [
                    {
                        ""id"": ""51356861-8ef6-44bb-af81-58d36b13943b"",
                        ""use"": ""official"",
                        ""system"": ""Patient Identifier"",
                        ""value"": ""GAN203007""
                    },
                    {
                        ""id"": ""4330e82a-679a-47d0-baa6-17a5bee302fd"",
                        ""use"": ""usual"",
                        ""system"": ""National ID"",
                        ""value"": ""NAT2805""
                    }
                ],
                ""active"": true,
                ""name"": [
                    {
                        ""id"": ""e54cd2af-b8f9-4d92-a234-00b11660814d"",
                        ""family"": ""Diabetes"",
                        ""given"": [
                            ""Test""
                        ]
                    }
                ],
                ""gender"": ""male"",
                ""birthDate"": ""1961-05-05"",
                ""deceasedBoolean"": false,
                ""address"": [
                    {
                        ""id"": ""cb5f7a58-a604-4534-8fc2-9f57ffed0468"",
                        ""extension"": [
                            {
                                ""url"": ""https://fhir.openmrs.org/ext/address"",
                                ""extension"": [
                                    {
                                        ""url"": ""https://fhir.openmrs.org/ext/address#address3"",
                                        ""valueString"": ""Kota""
                                    }
                                ]
                            }
                        ],
                        ""use"": ""home"",
                        ""city"": ""AAMAGOHAN"",
                        ""state"": ""Chattisgarh""
                    }
                ]
            }
        }
    ]
}")
                        })
                .Verifiable();

            //When
            var response = await openmrsClient.GetAsync("path/to/resource");
            //Then
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);

            var parser = new FhirJsonParser();

            var bundle = parser.Parse<Bundle>(content);
            var firstPatient = (Patient)bundle.Entry[0].Resource;
            firstPatient.Name[0].GivenElement.First().ToString().Should().Be("Test");
            firstPatient.Gender.Should().Be(AdministrativeGender.Male);
            firstPatient.BirthDate.Should().Be("1982-05-05");
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