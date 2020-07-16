using System.Linq;
using System.Net;
using System.Net.Http;
using FluentAssertions;
using Hl7.Fhir.Model;
using In.ProjectEKA.HipLibrary.Patient.Model;
using Moq;
using Xunit;

namespace In.ProjectEKA.HipServiceTest.OpenMrs
{
    [Collection("Discovery Data Source Tests")]
    public class DiscoveryDataSourceTest
    {
        [Fact]
        public async System.Threading.Tasks.Task ShouldReturnListOfPatientDto()
        {
            //Given
            var openmrsClientMock = new Mock<IOpenMrsClient>();
            var discoveryDataSource = new DiscoveryDataSource(openmrsClientMock.Object);

            openmrsClientMock.Setup(x => x.GetAsync("ws/fhir2/Patient")).ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(patientSample)
                })
                .Verifiable();

            //When
            var patients = await discoveryDataSource.LoadPatientsAsync(null, null, null);

            //Then
            var firstPatient = patients[0];
            firstPatient.Name[0].GivenElement.First().ToString().Should().Be("Test");
            firstPatient.Gender.Should().Be(AdministrativeGender.Female);
            firstPatient.BirthDate.Should().Be("1982-05-05");
            var secondPatient = patients[1];
            secondPatient.Name[0].GivenElement.First().ToString().Should().Be("David");
            secondPatient.Gender.Should().Be(AdministrativeGender.Male);
            secondPatient.BirthDate.Should().Be("1997-04-10");
        }

        [Fact]
        public async System.Threading.Tasks.Task ShouldReturnEmptyPatientWhenGotNoRecord()
        {
            //Given
            var openmrsClientMock = new Mock<IOpenMrsClient>();
            var discoveryDataSource = new DiscoveryDataSource(openmrsClientMock.Object);

            openmrsClientMock.Setup(x => x.GetAsync("ws/fhir2/Patient")).ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(notFoundData)
                })
                .Verifiable();

            //When
            var patients = await discoveryDataSource.LoadPatientsAsync(null, null, null);

            //Then
            patients.Count.Should().Be(0);
        }

        [Theory]
        [InlineData("ws/fhir2/Patient/?name=David", "David", null, null)]
        [InlineData("ws/fhir2/Patient/?gender=M", null, Gender.M, null)]
        [InlineData("ws/fhir2/Patient/?gender=F", null, Gender.F, null)]
        [InlineData("ws/fhir2/Patient/?birthdate=1982-05-05", null, null, "1982-05-05")]
        public async System.Threading.Tasks.Task ShouldQueryDataSourceByNameAccordingToTheFilter(
            string expectedPath, string name, Gender? gender, string yearOfBrith)
        {
            //Given
            var openmrsClientMock = new Mock<IOpenMrsClient>();
            var discoveryDataSource = new DiscoveryDataSource(openmrsClientMock.Object);

            openmrsClientMock
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(patientSample)
                })
                .Verifiable();

            //When
            var patients = await discoveryDataSource.LoadPatientsAsync(name, gender, yearOfBrith);

            //Then
            openmrsClientMock.Verify(client => client.GetAsync(expectedPath), Times.Once);
        }

        [Fact(Skip="Requires AWS access")]
        [Trait("Category", "Infrastructure")]
        public async System.Threading.Tasks.Task ShouldGetPatientDataRealCallAsync()
        {
            //Given
            // Disable SSL verification in test only
            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            var httpClient = new HttpClient(handler);
            var openmrsConfiguration = new OpenMrsConfiguration {
                Url = "https://bahmni-0.92.bahmni-covid19.in/openmrs/",
                Username = "superman",
                Password = "Admin123"
                };
            var openmrsClient = new OpenMrsClient(httpClient, openmrsConfiguration);
            var discoveryDataSource = new DiscoveryDataSource(openmrsClient);
            //When
            var patients = await discoveryDataSource.LoadPatientsAsync(null, null, null);
            //Then
            patients.Should().NotBeEmpty();
        }

        private const string patientSample = @"{
            ""resourceType"": ""Bundle"",
            ""id"": ""fbfee329-6108-4a7e-87b4-e047ae013c3a"",
            ""meta"": {
                ""lastUpdated"": ""2020-07-15T12:05:34.177+05:30""
            },
            ""type"": ""searchset"",
            ""total"": 2,
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
                        ""gender"": ""female"",
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
                                    ""David""
                                ]
                            }
                        ],
                        ""gender"": ""male"",
                        ""birthDate"": ""1997-04-10"",
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
        }";
        private const string notFoundData = @"{
            ""resourceType"": ""Bundle"",
            ""id"": ""aa12ec40-c9e8-40e9-9475-88a8fbab2793"",
            ""meta"": {
                ""lastUpdated"": ""2020-07-16T11:35:30.250+05:30""
            },
            ""type"": ""searchset"",
            ""total"": 0,
            ""link"": [
                {
                    ""relation"": ""self"",
                    ""url"": ""http://bahmni-0.92.bahmni-covid19.in/openmrs/ws/fhir2/Patient/?gender=O""
                }
            ]
        }";
    }
}