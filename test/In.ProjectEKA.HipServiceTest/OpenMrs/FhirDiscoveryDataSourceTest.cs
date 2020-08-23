using System.Linq;
using System.Net;
using System.Net.Http;
using FluentAssertions;
using Hl7.Fhir.Model;
using In.ProjectEKA.HipService.OpenMrs;
using Moq;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace In.ProjectEKA.HipServiceTest.OpenMrs
{
    [Collection("Fhir Discovery Data Source Tests")]
    public class FhirDiscoveryDataSourceTest
    {
        [Fact]
        public async Task ShouldReturnListOfPatientDto()
        {
            //Given
            var openmrsClientMock = new Mock<IOpenMrsClient>();
            var discoveryDataSource = new FhirDiscoveryDataSource(openmrsClientMock.Object);

            openmrsClientMock
                .Setup(x => x.GetAsync(Endpoints.Fhir.OnPatientPath))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(PatientSample)
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
        public async Task ShouldReturnEmptyListIfAllResourcesAreDifferentFromPatient()
        {
            //Given
            var openMrsClientMock = new Mock<IOpenMrsClient>();
            var discoveryDataSource = new FhirDiscoveryDataSource(openMrsClientMock.Object);

            openMrsClientMock
                .Setup(x => x.GetAsync(Endpoints.Fhir.OnPatientPath))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(PersonsSample)
                })
                .Verifiable();

            //When
            var patients = await discoveryDataSource.LoadPatientsAsync(null, null, null);

            //Then
            patients.Should().BeEmpty();
        }

        [Fact]
        public async Task ShouldReturnEmptyListWhenGotNoRecord()
        {
            //Given
            var openmrsClientMock = new Mock<IOpenMrsClient>();
            var discoveryDataSource = new FhirDiscoveryDataSource(openmrsClientMock.Object);

            openmrsClientMock
                .Setup(x => x.GetAsync(Endpoints.Fhir.OnPatientPath))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(NotFoundData)
                })
                .Verifiable();

            //When
            var patients = await discoveryDataSource.LoadPatientsAsync(null, null, null);

            //Then
            patients.Count.Should().Be(0);
            patients.Should().BeEmpty();
        }

        [Theory]
        [InlineData("ws/fhir2/Patient/?name=David%3f", "David?", null, null)]
        [InlineData("ws/fhir2/Patient/?name=David1", "David1", null, null)]
        [InlineData("ws/fhir2/Patient/?name=david", "david", null, null)]
        [InlineData("ws/fhir2/Patient", "", null, null)]
        [InlineData("ws/fhir2/Patient/?gender=male", null, AdministrativeGender.Male, null)]
        [InlineData("ws/fhir2/Patient/?gender=female", null, AdministrativeGender.Female, null)]
        [InlineData("ws/fhir2/Patient/?gender=unknown", null, AdministrativeGender.Unknown, null)]
        [InlineData("ws/fhir2/Patient/?gender=other", null, AdministrativeGender.Other, null)]
        [InlineData("ws/fhir2/Patient/?birthdate=1982-05-21", null, null, "1982-05-21")]
        [InlineData("ws/fhir2/Patient/?birthdate=1982", null, null, "1982")]
        [InlineData("ws/fhir2/Patient/?birthdate=1", null, null, "1")]
        [InlineData("ws/fhir2/Patient/?name=David&birthdate=1982-05-21", "David", null, "1982-05-21")]
        [InlineData("ws/fhir2/Patient/?name=David&gender=male&birthdate=1982-05-21", "David", AdministrativeGender.Male,
            "1982-05-21")]
        public async Task ShouldQueryDataSourceByNameAccordingToTheFilter(
            string expectedPath, string name, AdministrativeGender? gender, string yearOfBrith)
        {
            //Given
            var openmrsClientMock = new Mock<IOpenMrsClient>();
            var discoveryDataSource = new FhirDiscoveryDataSource(openmrsClientMock.Object);

            openmrsClientMock
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(PatientSample)
                })
                .Verifiable();

            //When
            var patients = await discoveryDataSource.LoadPatientsAsync(name, gender, yearOfBrith);

            //Then
            openmrsClientMock.Verify(client => client.GetAsync(expectedPath), Times.Once);
        }

        [Fact(Skip = "Requires AWS access")]
        [Trait("Category", "Infrastructure")]
        public async Task ShouldGetPatientDataRealCallAsync()
        {
            //Given
            // Disable SSL verification in test only
            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            var httpClient = new HttpClient(handler);
            var openmrsConfiguration = new OpenMrsConfiguration
            {
                Url = "https://someurl/openmrs/",
                Username = "someusername",
                Password = "somepassword"
            };
            var openmrsClient = new OpenMrsClient(httpClient, openmrsConfiguration);
            var discoveryDataSource = new FhirDiscoveryDataSource(openmrsClient);
            //When
            var patients = await discoveryDataSource.LoadPatientsAsync(null, null, null);
            //Then
            patients.Should().NotBeEmpty();
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadPatientAsync_ShouldReturnPatient_WhenFound() {
            //Given
            var openmrsClientMock = new Mock<IOpenMrsClient>();
            var discoveryDataSource = new FhirDiscoveryDataSource(openmrsClientMock.Object);
            var patientId = "95db77c6-cc8a-4208-99cc-c69a207114a3";
            var path = $"{Endpoints.Fhir.OnPatientPath}/{patientId}";

            openmrsClientMock
                .Setup(x => x.GetAsync(path))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(SamplePatient)
                })
                .Verifiable();

            //When
            var patient = await discoveryDataSource.LoadPatientAsync(patientId);

            //Then
            Assert.NotNull(patient);
            patient.Name[0].GivenElement.First().ToString().Should().Be("test");
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadPatientAsync_ShouldReturnError_WhenFound() {
            //Given
            var openmrsClientMock = new Mock<IOpenMrsClient>();
            var discoveryDataSource = new FhirDiscoveryDataSource(openmrsClientMock.Object);
            var invalidPatientId = "000000000";
            var path = $"{Endpoints.Fhir.OnPatientPath}/{invalidPatientId}";

            openmrsClientMock
                .Setup(x => x.GetAsync(path))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Content = new StringContent(PatientNotFoundData)
                })
                .Verifiable();

            //When
            var patient = await discoveryDataSource.LoadPatientAsync(invalidPatientId);

            //Then
           patient.Name.Should().BeEmpty();
           patient.Gender.Should().Be(null);
        }
        private const string PersonsSample = @"{
            ""resourceType"": ""Bundle"",
            ""id"": ""356ccc7d-fc38-4dab-809b-9efc6e33188a"",
            ""meta"": {
                ""lastUpdated"": ""2020-07-20T10:10:38.559+05:30""
            },
            ""type"": ""searchset"",
            ""total"": 2,
            ""link"": [
            {
                ""relation"": ""self"",
                ""url"": ""http://bahmni-0.92.bahmni-covid19.in/openmrs/ws/fhir2/Person/?name=Super""
            }
            ],
            ""entry"": [
            {
                ""resource"": {
                    ""resourceType"": ""Person"",
                    ""id"": ""4ec280ac-3f10-11e4-adec-0800271c1b75"",
                    ""name"": [
                    {
                        ""id"": ""4ec7e2a4-3f10-11e4-adec-0800271c1b75"",
                        ""family"": ""User"",
                        ""given"": [""Super""]
                    }
                    ],
                    ""gender"": ""male"",
                    ""active"": true
                }
            },
            {
                ""resource"": {
                    ""resourceType"": ""Person"",
                    ""id"": ""c1bc22a5-3f10-11e4-adec-0800271c1b75"",
                    ""meta"": {
                        ""lastUpdated"": ""2015-08-21T12:13:23.000+05:30""
                    },
                    ""name"": [
                    {
                        ""id"": ""c1bc22a5-3f10-11e4-adec-0800271c1b75"",
                        ""family"": ""Man"",
                        ""given"": [""Super""]
                    }
                    ],
                    ""gender"": ""male"",
                    ""active"": true
                },
            }
            ]
        }";

        private const string PatientSample = @"{
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

        private const string SamplePatient = @"{
            ""resourceType"": ""Patient"",
            ""id"": ""95db77c6-cc8a-4208-99cc-c69a207114a3"",
            ""identifier"": [
                {
                    ""id"": ""85e0ebdb-1984-4a6d-8137-2550b12939f1"",
                    ""use"": ""official"",
                    ""system"": ""Patient Identifier"",
                    ""value"": ""OD100001M""
                }
            ],
            ""active"": true,
            ""name"": [
                {
                    ""id"": ""fe860165-b614-4b20-b67e-78735a78c9d1"",
                    ""family"": ""patient"",
                    ""given"": [
                        ""test"",
                        ""one""
                    ]
                }
            ],
            ""gender"": ""male"",
            ""birthDate"": ""1977-04-04"",
            ""deceasedBoolean"": false,
            ""address"": [
                {
                    ""id"": ""e032e797-32c5-4a8d-b79c-f92c264977b4"",
                    ""extension"": [
                        {
                            ""url"": ""https://fhir.openmrs.org/ext/address"",
                            ""extension"": [
                                {
                                    ""url"": ""https://fhir.openmrs.org/ext/address#address1"",
                                    ""valueString"": ""Angul""
                                }
                            ]
                        }
                    ],
                    ""use"": ""home"",
                    ""state"": ""Odisha"",
                    ""country"": ""India""
                }
            ]
        }";
        private const string NotFoundData = @"{
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

        private const string PatientNotFoundData = @"{
            ""resourceType"": ""Patient""
        }";
    }
}