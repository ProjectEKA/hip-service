using System.Linq;
using System.Net;
using System.Net.Http;
using FluentAssertions;
using In.ProjectEKA.HipService.OpenMrs;
using Moq;
using Xunit;
using In.ProjectEKA.HipLibrary.Patient.Model;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace In.ProjectEKA.HipServiceTest.OpenMrs
{
    [Collection("OpenMrs Care Context Repository Tests")]
    public class OpenMrsCareContextRepositoryTest
    {
        private readonly Mock<IOpenMrsClient> openmrsClientMock;
        private readonly OpenMrsCareContextRepository careContextRepository;

        private const string programReferenceNumber = "c1720ca0-8ea3-4ef7-a4fa-a7849ab99d87";
        private const string programDisplayName = "HIV Program";

        public OpenMrsCareContextRepositoryTest()
        {
            openmrsClientMock = new Mock<IOpenMrsClient>();
            careContextRepository = new OpenMrsCareContextRepository(openmrsClientMock.Object);
        }

        [Fact]
        public async System.Threading.Tasks.Task ShouldReturnListOfProgramEnrollments()
        {
            //Given
            openMrsClientReturnsCareContexts(Endpoints.OpenMrs.OnProgramEnrollmentPath, ProgramEnrollmentSampleFull);

            //When
            var programenrollments = await careContextRepository.LoadProgramEnrollments(null);

            //Then
            var program = programenrollments[0];
            program.ReferenceNumber.Should().Be("12345");
            program.Display.Should().Be("HIV Program");
        }

        [Theory]
        [InlineData("\"attributes\":[],", ProgramDisplay)]
        [InlineData("", ProgramDisplay)]
        [InlineData(AttributesOfProgramEnrollmentWithoutValue, ProgramDisplay)]
        [InlineData(AttributesOfProgramEnrollment, "")]
        public void ShouldReturnErrorIfSomeFieldsAreMissing(string attribute, string display)
        {
            //Given
            var invalidSample = ProgramEnrollmentSample(attribute, display);
            openMrsClientReturnsCareContexts(Endpoints.OpenMrs.OnProgramEnrollmentPath, invalidSample);

            //When
            Func<Task> loadProgramEnrollments = async () => { await careContextRepository.LoadProgramEnrollments(null); };

            //Then
            loadProgramEnrollments.Should().Throw<OpenMrsFormatException>();
        }

        [Fact]
        public async System.Threading.Tasks.Task ShouldReturnEmptyListIfNoProgramEnrollmentsCareContexts()
        {
            //Given
            openMrsClientReturnsCareContexts(Endpoints.OpenMrs.OnProgramEnrollmentPath, EmptySample);

            //When
            var programenrollments = await careContextRepository.LoadProgramEnrollments(null);

            //Then
            programenrollments.Count().Should().Be(0);
        }

        [Fact]
        public async System.Threading.Tasks.Task ShouldReturnListOfVisits()
        {
            //Given
            openMrsClientReturnsCareContexts(Endpoints.OpenMrs.OnVisitPath, VisitSample);

            //When
            var visits = await careContextRepository.LoadVisits(null);

            //Then
            var visit = visits[0];
            visit.Display.Should().Be("OPD");
        }

        [Fact]
        public async System.Threading.Tasks.Task ShouldReturnListOfVisitsGroupedByType()
        {
            //Given
            openMrsClientReturnsCareContexts(Endpoints.OpenMrs.OnVisitPath, VisitSample);

            //When
            var visits = await careContextRepository.LoadVisits(null);

            //Then
            var numberOfVisitTypes = visits.Count();
            numberOfVisitTypes.Should().Be(2);
            var firstVisitType = visits[0];
            firstVisitType.Display.Should().Be("OPD");
            var secondVisitType = visits[1];
            secondVisitType.Display.Should().Be("Emergency");
        }

        [Fact]
        public async System.Threading.Tasks.Task ShouldReturnEmptyListIfNoVisitCareContexts()
        {
            //Given
            openMrsClientReturnsCareContexts(Endpoints.OpenMrs.OnVisitPath, EmptySample);

            //When
            var visits = await careContextRepository.LoadVisits(null);

            //Then
            visits.Count().Should().Be(0);
        }

        [Fact]
        public async System.Threading.Tasks.Task ShouldReturnCombinedListOfCareContexts()
        {
            //Given
            Mock<OpenMrsCareContextRepository> careContextRepository = new Mock<OpenMrsCareContextRepository>(openmrsClientMock.Object);
            careContextRepository.CallBase = true;
            careContextRepository
                .Setup(x => x.LoadProgramEnrollments(It.IsAny<string>()))
                .ReturnsAsync(
                    new List<CareContextRepresentation>
                    {
                        new CareContextRepresentation("12345", "HIV Program")
                    })
                .Verifiable();


            careContextRepository
                .Setup(x => x.LoadVisits(It.IsAny<string>()))
                .ReturnsAsync(
                    new List<CareContextRepresentation>
                    {
                        new CareContextRepresentation(null, "OPD"),
                        new CareContextRepresentation(null, "Emergency")
                    })
                .Verifiable();

            //When
            var combinedCareContexts =
                (await careContextRepository.Object.GetCareContexts(null)).ToList();

            //Then
            combinedCareContexts.Count().Should().Be(3);
            combinedCareContexts[0].ReferenceNumber.Should().Be("12345");
            combinedCareContexts[0].Display.Should().Be("HIV Program");

            combinedCareContexts[1].Display.Should().Be("OPD");
            combinedCareContexts[2].Display.Should().Be("Emergency");
        }

        private void openMrsClientReturnsCareContexts(string path, string response)
        {
            openmrsClientMock
                .Setup(x => x.GetAsync(path))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(response)
                })
                .Verifiable();
        }

        private string ProgramEnrollmentSampleFull = ProgramEnrollmentSample(AttributesOfProgramEnrollment, ProgramDisplay);

        private static Func<string, string, string> ProgramEnrollmentSample = (string attribute, string display) => @"{
            ""results"": [
                {
                    ""uuid"": ""c1720ca0-8ea3-4ef7-a4fa-a7849ab99d87"",
                    ""patient"": {
                        ""uuid"": ""b5e712bc-9472-41c0-a11f-500deac452d2"",
                        ""display"": ""1234 - John Doe"",
                        ""identifiers"": [
                            {
                                ""uuid"": ""af8996cb-e94b-463c-9ba0-17630dc12e0b"",
                                ""display"": ""Patient Identifier = 1234"",
                                ""links"": [
                                    {
                                        ""rel"": ""self"",
                                        ""uri"": ""http://192.168.33.10/openmrs/ws/rest/v1/patient/b5e712bc-9472-41c0-a11f-500deac452d2/identifier/af8996cb-e94b-463c-9ba0-17630dc12e0b""
                                    }
                                ]
                            }
                        ]
                    }," + attribute + @"
                    ""program"": {
                        ""name"": ""HIV Program"",
                        ""uuid"": ""5789a170-c020-4879-ae39-06b1de26cb5f"",
                        ""retired"": false,
                        ""description"": ""HIV Program"",
                        ""concept"": {
                            ""uuid"": ""ec41264d-e82e-4356-a7d2-7c1ff6c90abe"",
                            ""display"": ""HIV"",
                            ""links"": [
                                {
                                    ""rel"": ""self"",
                                    ""uri"": ""http://192.168.33.10/openmrs/ws/rest/v1/concept/ec41264d-e82e-4356-a7d2-7c1ff6c90abe""
                                }
                            ]
                        }
                    }," + display + @"
                    ""dateEnrolled"": ""2020-07-13T14:00:00.000+0000"",
                    ""dateCompleted"": null,
                    ""location"": null,
                    ""voided"": false,
                    ""outcome"": null,
                    ""links"": [
                        {
                            ""rel"": ""self"",
                            ""uri"": ""http://192.168.33.10/openmrs/ws/rest/v1/bahmniprogramenrollment/c1720ca0-8ea3-4ef7-a4fa-a7849ab99d87""
                        }
                    ],
                    ""resourceVersion"": ""1.8""
                }
            ]
        }";

        private const string AttributesOfProgramEnrollmentWithoutValue = @"""attributes"": [
                        {
                            ""display"": ""ID_Number: 12345""
                        }
                    ],";

        private const string AttributesOfProgramEnrollment = @"""attributes"": [
                        {
                            ""display"": ""ID_Number: 12345"",
                            ""uuid"": ""11d5bc55-b94c-480c-ac0b-e5c9a7e40c20"",
                            ""attributeType"": {
                                ""uuid"": ""c41f844e-a707-11e6-91e9-0800270d80ce"",
                                ""display"": ""ID_Number"",
                                ""description"": ""ID Number"",
                                ""retired"": false,
                                ""links"": [
                                    {
                                        ""rel"": ""self"",
                                        ""uri"": ""http://192.168.33.10/openmrs/ws/rest/v1/programattributetype/c41f844e-a707-11e6-91e9-0800270d80ce""
                                    }
                                ]
                            },
                            ""value"": ""12345"",
                            ""voided"": false,
                            ""links"": [
                                {
                                    ""rel"": ""self"",
                                    ""uri"": ""http://192.168.33.10/openmrs/ws/rest/v1/bahmniprogramenrollment/57f76d2d-358e-4135-8160-247a94c49535/attribute/11d5bc55-b94c-480c-ac0b-e5c9a7e40c20""
                                },
                                {
                                    ""rel"": ""full"",
                                    ""uri"": ""http://192.168.33.10/openmrs/ws/rest/v1/bahmniprogramenrollment/57f76d2d-358e-4135-8160-247a94c49535/attribute/11d5bc55-b94c-480c-ac0b-e5c9a7e40c20?v=full""
                                }
                            ],
                            ""resourceVersion"": ""1.9""
                        }
                    ],";

        private const string ProgramDisplay = @"""display"": ""HIV Program"",";

        private const string VisitSample = @"{
            ""results"": [
                {
                    ""uuid"": ""fd377423-c804-4df2-b340-1ef844206769"",
                    ""display"": ""OPD @ Odisha - 07/22/2020 11:28 AM"",
                    ""patient"": {
                        ""uuid"": ""2ceb3abb-1724-4f4f-b969-43b63f73545e"",
                        ""display"": ""OD100012 - Anshul Test One"",
                        ""links"": [
                            {
                                ""rel"": ""self"",
                                ""uri"": ""http://bahmni-0.92.bahmni-covid19.in/openmrs/ws/rest/v1/patient/2ceb3abb-1724-4f4f-b969-43b63f73545e""
                            }
                        ]
                    },
                    ""visitType"": {
                        ""uuid"": ""96c49059-9af1-4f63-a8be-7554984fda02"",
                        ""display"": ""OPD"",
                        ""links"": [
                            {
                                ""rel"": ""self"",
                                ""uri"": ""http://bahmni-0.92.bahmni-covid19.in/openmrs/ws/rest/v1/visittype/96c49059-9af1-4f63-a8be-7554984fda02""
                            }
                        ]
                    }
                },
                {
                    ""uuid"": ""fd377423-c804-4df2-b340-1ef844206769"",
                    ""display"": ""OPD @ Odisha - 07/22/2020 11:28 AM"",
                    ""patient"": {
                        ""uuid"": ""2ceb3abb-1724-4f4f-b969-43b63f73545e"",
                        ""display"": ""OD100012 - Anshul Test One"",
                        ""links"": [
                            {
                                ""rel"": ""self"",
                                ""uri"": ""http://bahmni-0.92.bahmni-covid19.in/openmrs/ws/rest/v1/patient/2ceb3abb-1724-4f4f-b969-43b63f73545e""
                            }
                        ]
                    },
                    ""visitType"": {
                        ""uuid"": ""96c49059-9af1-4f63-a8be-7554984fda02"",
                        ""display"": ""OPD"",
                        ""links"": [
                            {
                                ""rel"": ""self"",
                                ""uri"": ""http://bahmni-0.92.bahmni-covid19.in/openmrs/ws/rest/v1/visittype/96c49059-9af1-4f63-a8be-7554984fda02""
                            }
                        ]
                    }
                },
                {
                    ""uuid"": ""fd377423-c804-4df2-b340-1ef844206769"",
                    ""display"": ""OPD @ Odisha - 07/22/2020 11:28 AM"",
                    ""patient"": {
                        ""uuid"": ""2ceb3abb-1724-4f4f-b969-43b63f73545e"",
                        ""display"": ""OD100012 - Anshul Test One"",
                        ""links"": [
                            {
                                ""rel"": ""self"",
                                ""uri"": ""http://bahmni-0.92.bahmni-covid19.in/openmrs/ws/rest/v1/patient/2ceb3abb-1724-4f4f-b969-43b63f73545e""
                            }
                        ]
                    },
                    ""visitType"": {
                        ""uuid"": ""96c49059-9af1-4f63-a8be-7554984fda02"",
                        ""display"": ""Emergency"",
                        ""links"": [
                            {
                                ""rel"": ""self"",
                                ""uri"": ""http://bahmni-0.92.bahmni-covid19.in/openmrs/ws/rest/v1/visittype/96c49059-9af1-4f63-a8be-7554984fda02""
                            }
                        ]
                    }
                }
            ]
        }";

        private const string EmptySample = @"{
            ""results"": []
        }";
    }
}