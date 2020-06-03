using System;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using In.ProjectEKA.TMHHip.DataFlow;
using In.ProjectEKA.TMHHip.DataFlow.Model;
using In.ProjectEKA.TMHHip.Link;
using Moq.Protected;
using Newtonsoft.Json;
using In.ProjectEKA.HipLibrary.Patient.Model;

namespace In.ProjectEKA.TMHHipTest.DataFlow
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using HipLibrary.Patient.Model;
    using Optional.Unsafe;
    using Xunit;
    using Moq;


    [Collection("Collect Tests")]
    public class CollectTest
    {
        [Fact]
        private async void ReturnEntries()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Loose);
            var repoMock = new Mock<HttpMessageHandler>(MockBehavior.Loose);
            var tmhClient = new HttpClient(handlerMock.Object);
            var repoClient = new HttpClient(repoMock.Object);
            var patientRepository = new PatientRepository(repoClient);
            var collect = new Collect(tmhClient, patientRepository);
            var noteCreatedTime = new DateTime(2018, 1, 1);
            var clinicalNote = new ClinicalNote
                {CreatedDate = noteCreatedTime, Note = "some note", NoteNumber = 1, UserName = "doctor"};
            var clinicalNoteResponse = JsonConvert.SerializeObject(new List<ClinicalNote> {clinicalNote});

            var patient = new TMHHip.Discovery.Patient
            {
                DateOfBirth = new DateTime(), FirstName = "test", Gender = "F", Identifier = "MOBILE",
                LastName = "test",
                PhoneNumber = "9999999999"
            };
            var patientResponse = JsonConvert.SerializeObject(patient);

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(clinicalNoteResponse, Encoding.UTF8, MediaTypeNames.Application.Json)
                })
                .Verifiable();


            repoMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(patientResponse, Encoding.UTF8, MediaTypeNames.Application.Json)
                })
                .Verifiable();


            const string consentId = "ConsentId";
            const string consentManagerId = "ConsentManagerId";
            var grantedContexts = new List<GrantedContext>
            {
                new GrantedContext("RVH1003", "BI-KTH-12.05.0024"),
                new GrantedContext("RVH1003", "NCP1008")
            };
            var dateRange = new DateRange("2017-12-01T15:43:00.000+0000", "2020-03-31T15:43:19.279+0000");
            var hiTypes = new List<HiType>
            {
                HiType.Condition,
                HiType.Observation,
                HiType.DiagnosticReport,
                HiType.MedicationRequest
            };
            var dataRequest = new DataRequest(grantedContexts,
                dateRange,
                "/someUrl",
                hiTypes,
                "someTxnId",
                null,
                consentManagerId,
                consentId);

            var entries = await collect.CollectData(dataRequest);
            entries.ValueOrDefault().CareBundles.Count().Should().Be(1);
        }
    }
}