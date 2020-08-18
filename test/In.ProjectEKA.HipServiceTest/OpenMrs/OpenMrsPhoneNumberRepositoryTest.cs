using System.Net;
using System.Net.Http;
using FluentAssertions;
using In.ProjectEKA.HipService.OpenMrs;
using Moq;
using Xunit;

namespace In.ProjectEKA.HipServiceTest.OpenMrs
{
    [Collection("OpenMrs Phone Number Repository Tests")]
    public class OpenMrsPhoneNumberRepositoryTest
    {
        [Fact]
        public async System.Threading.Tasks.Task GetPhoneNumber_ShouldExtractSecondaryContactFromPatientDataAsPhoneNumber_WhenSpecifyPatientId() {
            //Given
            var openmrsClientMock = new Mock<IOpenMrsClient>();
            var repository = new OpenMrsPhoneNumberRepository(openmrsClientMock.Object);
            var patientId = "some-patient-id";
            var path = $"{Endpoints.OpenMrs.OnPatientPath}/{patientId}";

            openmrsClientMock
                .Setup(x => x.GetAsync(path))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(OpenMrsRestPatientSampleWithSecondaryContact)
                })
                .Verifiable();

            //When
            var phoneNumber = await repository.GetPhoneNumber(patientId);

            //Then
            phoneNumber.Should().Be("+91-9999999999");
        }

        [Fact]
        public async System.Threading.Tasks.Task GetPhoneNumber_ShouldReturnNull_WhenThereIsNoSecondaryPhoneNumber() {
            //Given
            var openmrsClientMock = new Mock<IOpenMrsClient>();
            var repository = new OpenMrsPhoneNumberRepository(openmrsClientMock.Object);
            var patientId = "some-patient-id";
            var path = $"{Endpoints.OpenMrs.OnPatientPath}/{patientId}";

            openmrsClientMock
                .Setup(x => x.GetAsync(path))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(OpenMrsRestPatientSampleWithoutSecondaryContact)
                })
                .Verifiable();

            //When
            var phoneNumber = await repository.GetPhoneNumber(patientId);

            //Then
            phoneNumber.Should().Be(null);
        }

        private const string OpenMrsRestPatientSampleWithSecondaryContact = @"{
            ""uuid"": ""81bcb347-3ed7-4b8e-a0fd-5b72eabed934"",
            ""display"": ""GAN203006 - Test Test"",
            ""identifiers"": [
                {
                    ""uuid"": ""460a1b0e-dca9-4535-ae61-f3152adcb4fb"",
                    ""display"": ""Patient Identifier = GAN203006"",
                    ""links"": [
                        {
                            ""rel"": ""self"",
                            ""uri"": ""http://qa-01.bahmni-covid19.in/openmrs/ws/rest/v1/patient/81bcb347-3ed7-4b8e-a0fd-5b72eabed934/identifier/460a1b0e-dca9-4535-ae61-f3152adcb4fb""
                        }
                    ]
                },
                {
                    ""uuid"": ""88289d1c-0d04-4272-8426-fba972f29d3c"",
                    ""display"": ""National ID = NAT2804"",
                    ""links"": [
                        {
                            ""rel"": ""self"",
                            ""uri"": ""http://qa-01.bahmni-covid19.in/openmrs/ws/rest/v1/patient/81bcb347-3ed7-4b8e-a0fd-5b72eabed934/identifier/88289d1c-0d04-4272-8426-fba972f29d3c""
                        }
                    ]
                }
            ],
            ""person"": {
                ""uuid"": ""81bcb347-3ed7-4b8e-a0fd-5b72eabed934"",
                ""display"": ""Test Test"",
                ""gender"": ""M"",
                ""age"": 24,
                ""birthdate"": ""1996-04-13T00:00:00.000+0530"",
                ""birthdateEstimated"": false,
                ""dead"": false,
                ""deathDate"": null,
                ""causeOfDeath"": null,
                ""preferredName"": {
                    ""uuid"": ""447b0091-e6bb-4ae6-992e-6f0ce986ad36"",
                    ""display"": ""Test Test"",
                    ""links"": [
                        {
                            ""rel"": ""self"",
                            ""uri"": ""http://qa-01.bahmni-covid19.in/openmrs/ws/rest/v1/person/81bcb347-3ed7-4b8e-a0fd-5b72eabed934/name/447b0091-e6bb-4ae6-992e-6f0ce986ad36""
                        }
                    ]
                },
                ""preferredAddress"": {
                    ""uuid"": ""4432698d-d477-4bd1-b014-fe9c56f30c39"",
                    ""display"": null,
                    ""links"": [
                        {
                            ""rel"": ""self"",
                            ""uri"": ""http://qa-01.bahmni-covid19.in/openmrs/ws/rest/v1/person/81bcb347-3ed7-4b8e-a0fd-5b72eabed934/address/4432698d-d477-4bd1-b014-fe9c56f30c39""
                        }
                    ]
                },
                ""attributes"": [
                    {
                        ""uuid"": ""852a9e8d-c956-4a03-a310-98365f49ad5a"",
                        ""display"": ""General"",
                        ""links"": [
                            {
                                ""rel"": ""self"",
                                ""uri"": ""http://qa-01.bahmni-covid19.in/openmrs/ws/rest/v1/person/81bcb347-3ed7-4b8e-a0fd-5b72eabed934/attribute/852a9e8d-c956-4a03-a310-98365f49ad5a""
                            }
                        ]
                    },
                    {
                        ""uuid"": ""df4af84e-bc10-4620-b8f0-df1a355cba54"",
                        ""display"": ""secondaryContact = +91-9999999999"",
                        ""links"": [
                            {
                                ""rel"": ""self"",
                                ""uri"": ""http://qa-01.bahmni-covid19.in/openmrs/ws/rest/v1/person/81bcb347-3ed7-4b8e-a0fd-5b72eabed934/attribute/df4af84e-bc10-4620-b8f0-df1a355cba54""
                            }
                        ]
                    },
                    {
                        ""uuid"": ""a190a81d-1b75-411e-a358-779e62301dbb"",
                        ""display"": ""landHolding = 2"",
                        ""links"": [
                            {
                                ""rel"": ""self"",
                                ""uri"": ""http://qa-01.bahmni-covid19.in/openmrs/ws/rest/v1/person/81bcb347-3ed7-4b8e-a0fd-5b72eabed934/attribute/a190a81d-1b75-411e-a358-779e62301dbb""
                            }
                        ]
                    }
                ],
                ""voided"": false,
                ""deathdateEstimated"": false,
                ""birthtime"": null,
                ""links"": [
                    {
                        ""rel"": ""self"",
                        ""uri"": ""http://qa-01.bahmni-covid19.in/openmrs/ws/rest/v1/person/81bcb347-3ed7-4b8e-a0fd-5b72eabed934""
                    },
                    {
                        ""rel"": ""full"",
                        ""uri"": ""http://qa-01.bahmni-covid19.in/openmrs/ws/rest/v1/person/81bcb347-3ed7-4b8e-a0fd-5b72eabed934?v=full""
                    }
                ],
                ""resourceVersion"": ""1.11""
            },
            ""voided"": false,
            ""links"": [
                {
                    ""rel"": ""self"",
                    ""uri"": ""http://qa-01.bahmni-covid19.in/openmrs/ws/rest/v1/patient/81bcb347-3ed7-4b8e-a0fd-5b72eabed934""
                },
                {
                    ""rel"": ""full"",
                    ""uri"": ""http://qa-01.bahmni-covid19.in/openmrs/ws/rest/v1/patient/81bcb347-3ed7-4b8e-a0fd-5b72eabed934?v=full""
                }
            ],
            ""resourceVersion"": ""1.8""
        }";

        private const string OpenMrsRestPatientSampleWithoutSecondaryContact = @"{
            ""person"": {
                ""attributes"": []
            }
        }";
    }
}