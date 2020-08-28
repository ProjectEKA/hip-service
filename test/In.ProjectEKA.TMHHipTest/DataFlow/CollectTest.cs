using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using In.ProjectEKA.HipLibrary.Patient.Model;
using In.ProjectEKA.TMHHip.DataFlow;
using In.ProjectEKA.TMHHip.DataFlow.Model;
using In.ProjectEKA.TMHHip.Link;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Optional.Unsafe;
using Xunit;
using Patient = In.ProjectEKA.TMHHip.Discovery.Patient;

namespace In.ProjectEKA.TMHHipTest.DataFlow
{
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

            var patient = new Patient
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
                consentId, "ncg");

            var entries = await collect.CollectData(dataRequest);
            entries.ValueOrDefault().CareBundles.Count().Should().Be(1);
        }

        [Fact]
        private async void ReturnDiagnosticReportAsPdfWithUrlRedirect()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Loose);
            var repoMock = new Mock<HttpMessageHandler>(MockBehavior.Loose);
            var tmhClient = new HttpClient(handlerMock.Object);
            var repoClient = new HttpClient(repoMock.Object);
            var patientRepository = new PatientRepository(repoClient);
            var collect = new Collect(tmhClient, patientRepository);
            var dateObj = new DateTime(2018, 1, 1);
            var diagReport = new DiagnosticReportAsPdf
            {
                ReportText = "reportText",
                Issued = dateObj,
                Performer = "performer",
                ContentType = "application/pdf",
                ReportConclusion = "conclusion",
                ReportTitle = "title",
                ReportUrl = "https://tmc.gov.in/EMRREPORTS1/allreports1.aspx?reqno=JfMcCHl8H2Yx6CfwfckII4IUQzG9QChzqJQ8BGLQPdw5HGovDusYmyKH1gbRQc8t",
                EffectiveDateTime = dateObj
            };
            var diagReports = new List<DiagnosticReportAsPdf> {diagReport};

            var patientDataResponse =
                JsonConvert.SerializeObject(new PatientData {DiagnosticReportAsPdf = diagReports});

            var patient = new Patient
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
            var dateRange = new DateRange("2000-12-01T15:43:00", "2022-03-31T15:43:19");
            var hiTypes = new List<HiType>
            {
                HiType.DiagnosticReport
            };
            var dataRequest = new DataRequest(grantedContexts,
                dateRange,
                "/someUrl",
                hiTypes,
                "someTxnId",
                null,
                consentManagerId,
                consentId, "ncg");

            var entries = await collect.CollectData(dataRequest);
            entries.ValueOrDefault().CareBundles.Count().Should().Be(1);
        }

        [Fact]
        private async void ReturnDiagnosticReportAsPdfWithoutRedirect()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Loose);
            var repoMock = new Mock<HttpMessageHandler>(MockBehavior.Loose);
            var tmhClient = new HttpClient(handlerMock.Object);
            var repoClient = new HttpClient(repoMock.Object);
            var patientRepository = new PatientRepository(repoClient);
            var collect = new Collect(tmhClient, patientRepository);
            var dateObj = new DateTime(2018, 1, 1);
            var diagReport = new DiagnosticReportAsPdf
            {
                ReportText = "reportText",
                Issued = dateObj,
                Performer = "performer",
                ContentType = "application/pdf",
                ReportConclusion = "conclusion",
                ReportTitle = "title",
                ReportUrl = "http://kirpalsingh.org/Booklets/Path_of_the_Masters.pdf",
                EffectiveDateTime = dateObj
            };
            var diagReports = new List<DiagnosticReportAsPdf> {diagReport};

            var patientDataResponse =
                JsonConvert.SerializeObject(new PatientData {DiagnosticReportAsPdf = diagReports});

            var patient = new Patient
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
            var dateRange = new DateRange("2000-12-01T15:43:00", "2022-03-31T15:43:19");
            var hiTypes = new List<HiType>
            {
                HiType.DiagnosticReport
            };
            var dataRequest = new DataRequest(grantedContexts,
                dateRange,
                "/someUrl",
                hiTypes,
                "someTxnId",
                null,
                consentManagerId,
                consentId, "ncg");

            var entries = await collect.CollectData(dataRequest);
            entries.ValueOrDefault().CareBundles.Count().Should().Be(1);
        }

        [Fact]
        private async void ReturnAbdomenExaminationData()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Loose);
            var repoMock = new Mock<HttpMessageHandler>(MockBehavior.Loose);
            var tmhClient = new HttpClient(handlerMock.Object);
            var repoClient = new HttpClient(repoMock.Object);
            var patientRepository = new PatientRepository(repoClient);
            var collect = new Collect(tmhClient, patientRepository);
            var abdomenExaminationData = new AbdomenExaminationData
            {
                CAbdomen = "some notes",
                CaseNumber = "case number",
                EffectiveDateTime = new DateTime(2018, 10, 3),
                HstExmNo = 1
            };
            var abdomenExaminationsData = new List<AbdomenExaminationData> {abdomenExaminationData};
            var patientData = new PatientData
            {
                AbdomenExaminationsData = abdomenExaminationsData
            };
            var patientDataResponse = JsonConvert.SerializeObject(patientData);

            var patient = new Patient
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
                consentId, "ncg");

            var entries = await collect.CollectData(dataRequest);
            entries.ValueOrDefault().CareBundles.Count().Should().Be(1);
        }

        [Fact]
        private async void ReturnSurgeryHistories()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Loose);
            var repoMock = new Mock<HttpMessageHandler>(MockBehavior.Loose);
            var tmhClient = new HttpClient(handlerMock.Object);
            var repoClient = new HttpClient(repoMock.Object);
            var patientRepository = new PatientRepository(repoClient);
            var collect = new Collect(tmhClient, patientRepository);
            var surgeryHistoryData = new SurgeryHistory
            {
                CaseNumber = "case number",
                HospitalDtls = "TMH",
                SurgeryDtls = "surgery details",
                SurgeryRemarks = "remarks",
                SurgeryWhen = new DateTime(2018, 10, 3),
                HstExmNo = 1
            };
            var surgeryHistories = new List<SurgeryHistory> {surgeryHistoryData};
            var patientData = new PatientData
            {
                SurgeryHistories = surgeryHistories
            };
            var patientDataResponse = JsonConvert.SerializeObject(patientData);

            var patient = new Patient
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
                consentId, "ncg");

            var entries = await collect.CollectData(dataRequest);
            entries.ValueOrDefault().CareBundles.Count().Should().Be(1);
        }

        [Fact]
        private async void ReturnAllergiesData()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Loose);
            var repoMock = new Mock<HttpMessageHandler>(MockBehavior.Loose);
            var tmhClient = new HttpClient(handlerMock.Object);
            var repoClient = new HttpClient(repoMock.Object);
            var patientRepository = new PatientRepository(repoClient);
            var collect = new Collect(tmhClient, patientRepository);
            var allergyData = new AllergyData
            {
                CaseNumber = "case number",
                HstExmNo = 1,
                Allergies = "allergies",
                AllergyDate = new DateTime(2018, 10, 3),
                AllergyRemark = "allergy remarks"
            };
            var allergiesData = new List<AllergyData> {allergyData};
            var patientData = new PatientData
            {
                AllergiesData = allergiesData
            };
            var patientDataResponse = JsonConvert.SerializeObject(patientData);

            var patient = new Patient
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
                consentId, "ncg");

            var entries = await collect.CollectData(dataRequest);
            entries.ValueOrDefault().CareBundles.Count().Should().Be(1);
        }

        [Fact]
        private async void ReturnSwellingSymptomsData()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Loose);
            var repoMock = new Mock<HttpMessageHandler>(MockBehavior.Loose);
            var tmhClient = new HttpClient(handlerMock.Object);
            var repoClient = new HttpClient(repoMock.Object);
            var patientRepository = new PatientRepository(repoClient);
            var collect = new Collect(tmhClient, patientRepository);
            var swellingSymptomData = new SwellingSymptomData
            {
                CaseNumber = "case number",
                RecordedDate = new DateTime(2018, 10, 3),
                SwellingLtrl = "ltrl",
                SwellingSite = "site",
                SwellingSize = "size",
                SymptNo = 1,
                HstExmNo = 1
            };
            var swellingSymptomsData = new List<SwellingSymptomData> {swellingSymptomData};
            var patientData = new PatientData
            {
                SwellingSymptomsData = swellingSymptomsData
            };
            var patientDataResponse = JsonConvert.SerializeObject(patientData);

            var patient = new Patient
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
                HiType.Condition
            };
            var dataRequest = new DataRequest(grantedContexts,
                dateRange,
                "/someUrl",
                hiTypes,
                "someTxnId",
                null,
                consentManagerId,
                consentId, "ncg");

            var entries = await collect.CollectData(dataRequest);
            entries.ValueOrDefault().CareBundles.Count().Should().Be(1);
        }

        [Fact]
        private async void ReturnOralCavityExaminationData()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Loose);
            var repoMock = new Mock<HttpMessageHandler>(MockBehavior.Loose);
            var tmhClient = new HttpClient(handlerMock.Object);
            var repoClient = new HttpClient(repoMock.Object);
            var patientRepository = new PatientRepository(repoClient);
            var collect = new Collect(tmhClient, patientRepository);
            var oralCavityExaminationData = new OralCavityExaminationData
            {
                Buccalmcsa = "buccalmcsa",
                Tongue = "tongue",
                CaseNumber = "case number",
                EffectiveDateTime = new DateTime(2018, 10, 3),
                HstExmNo = 1
            };
            var oralCavityExaminationsData = new List<OralCavityExaminationData> {oralCavityExaminationData};
            var patientData = new PatientData
            {
                OralCavityExaminationsData = oralCavityExaminationsData
            };
            var patientDataResponse = JsonConvert.SerializeObject(patientData);

            var patient = new Patient
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
                consentId, "@ncg");

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
                {Prescriptions = new List<Prescription> {prescription}};
            var patientDataResponse = JsonConvert.SerializeObject(patientData);

            var patient = new Patient
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
                consentId, "@ncg");

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

            var patient = new Patient
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
                consentId, "@ncg");

            var entries = await collect.CollectData(dataRequest);
            entries.ValueOrDefault().CareBundles.Count().Should().Be(2);
        }
    }
}