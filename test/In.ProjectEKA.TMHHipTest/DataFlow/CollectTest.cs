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
        private async void ReturnClinicalNotes()
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
            var clinicalNotes = new List<ClinicalNote> {clinicalNote};
            var patientData = new PatientData {ClinicalNotes = clinicalNotes, Prescriptions = null};
            var patientDataResponse = JsonConvert.SerializeObject(patientData);

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
                    Content = new StringContent(patientDataResponse, Encoding.UTF8, MediaTypeNames.Application.Json)
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
            var dateRange = new DateRange("2017-12-01T15:43:00.000", "2020-03-31T15:43:19.279");
            var hiTypes = new List<HiType>
            {
                HiType.Observation
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

        [Fact]
        private async void ReturnPrescriptions()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Loose);
            var repoMock = new Mock<HttpMessageHandler>(MockBehavior.Loose);
            var tmhClient = new HttpClient(handlerMock.Object);
            var repoClient = new HttpClient(repoMock.Object);
            var patientRepository = new PatientRepository(repoClient);
            var collect = new Collect(tmhClient, patientRepository);
            var date = new DateTime(2018, 1, 1);
            var prescription = new Prescription
            {
                Date = date,
                Dosage = "1-1-1",
                Medicine = "NARCODOL",
                CaseNumber = "caseNumber",
                GivenQuantity = 1,
                GenName = "GenName",
                PrescriptionId = "prescriptionId",
                RequiredQuantity = 1
            };
            var patientData = new PatientData
                {ClinicalNotes = null, Prescriptions = new List<Prescription> {prescription}};
            var patientDataResponse = JsonConvert.SerializeObject(patientData);

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
                    Content = new StringContent(patientDataResponse, Encoding.UTF8, MediaTypeNames.Application.Json)
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
            var dateRange = new DateRange("2017-12-01T15:43:00.000", "2020-03-31T15:43:19.279");
            var hiTypes = new List<HiType>
            {
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

        [Fact]
        private async void ReturnPrescriptionsAndClinicalNotes()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Loose);
            var repoMock = new Mock<HttpMessageHandler>(MockBehavior.Loose);
            var tmhClient = new HttpClient(handlerMock.Object);
            var repoClient = new HttpClient(repoMock.Object);
            var patientRepository = new PatientRepository(repoClient);
            var collect = new Collect(tmhClient, patientRepository);
            var date = new DateTime(2018, 1, 1);
            var noteCreatedTime = new DateTime(2018, 1, 1);
            var clinicalNote = new ClinicalNote
                {CreatedDate = noteCreatedTime, Note = "some note", NoteNumber = 1, UserName = "doctor"};
            var clinicalNotes = new List<ClinicalNote> {clinicalNote};
            var prescription = new Prescription
            {
                Date = date,
                Dosage = "1-1-1",
                Medicine = "NARCODOL",
                CaseNumber = "caseNumber",
                GivenQuantity = 1,
                GenName = "GenName",
                PrescriptionId = "prescriptionId",
                RequiredQuantity = 1
            };
            var patientData = new PatientData
                {ClinicalNotes = clinicalNotes, Prescriptions = new List<Prescription> {prescription}};
            var patientDataResponse = JsonConvert.SerializeObject(patientData);

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
                    Content = new StringContent(patientDataResponse, Encoding.UTF8, MediaTypeNames.Application.Json)
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
            var dateRange = new DateRange("2017-12-01T15:43:00.000", "2020-03-31T15:43:19.279");
            var hiTypes = new List<HiType>
            {
                HiType.MedicationRequest,
                HiType.Observation
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
            entries.ValueOrDefault().CareBundles.Count().Should().Be(2);
        }
    }
}