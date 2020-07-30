namespace In.ProjectEKA.HipServiceTest.Discovery
{
    using System;
    using FluentAssertions;
    using HipLibrary.Patient.Model;
    using Xunit;
    using Moq;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using OpenMrsPatient = Hl7.Fhir.Model.Patient;
    using OpenMrsPatientName = Hl7.Fhir.Model.HumanName;
    using OpenMrsGender = Hl7.Fhir.Model.AdministrativeGender;
    using In.ProjectEKA.HipService.OpenMrs;

    public class OpenMrsPatientMatchingRepositoryTest
    {
        private Mock<IPatientDal> patientDal = new Mock<IPatientDal>();

        public OpenMrsPatientMatchingRepositoryTest()
        {
                patientDal.Setup(e =>
                    e
                        .LoadPatientsAsync(
                            It.IsAny<string>(),
                            It.IsAny<OpenMrsGender?>(),
                            It.IsAny<string>()))
                        .Returns(
                            (string name, OpenMrsGender? gender, string yob) => {
                                var humanName = new OpenMrsPatientName();
                                humanName.Text = $"OpenMRS name of {name}";

                                return Task.FromResult(
                                    new List<OpenMrsPatient>() {
                                        new OpenMrsPatient() {
                                            Name = new List<OpenMrsPatientName>{ humanName },
                                            Gender = OpenMrsGender.Female,
                                            BirthDate = "1981"
                                        }
                                    });
                        });
        }

        [Fact]
        private async void PatientRepositoryWhereQuery_InvokesPatientDalWithExpectedParameters()
        {
            const string  patientName = "patient name";
            Gender? patientGender = Gender.F;
            OpenMrsGender? openMrsGender = OpenMrsGender.Female;

            ushort?  patientYob = 1981;
            var patientEnquiry =
                new PatientEnquiry(
                    "id", verifiedIdentifiers: null, unverifiedIdentifiers: null,
                    patientName, patientGender, patientYob);
            var request = new DiscoveryRequest(patientEnquiry,"requestId", "transactionId", DateTime.Now);
            var repo = new OpenMrsPatientMatchingRepository(patientDal.Object);
           
            var result = repo.Where(request);

            patientDal.Verify( x => x.LoadPatientsAsync(patientName, openMrsGender, patientYob.ToString()), Times.Once);

        }

        [Fact]
        private async void PatientRepositoryWhereQuery_ReturnsAnHIPPatientWithExpectedValues_WhenPatientFoundInOpenMrs()
        {
            const string  patientSearchedName = "patient name";
            Gender? patientGender = Gender.F;

            ushort?  patientYob = 1981;
            var patientEnquiry =
                new PatientEnquiry(
                    "id", verifiedIdentifiers: null, unverifiedIdentifiers: null,
                    patientSearchedName, patientGender, patientYob);
            var request = new DiscoveryRequest(patientEnquiry,"requestId", "transactionId", DateTime.Now);
            var repo = new OpenMrsPatientMatchingRepository(patientDal.Object);

            var patient = repo.Where(request).Result.Single();

            patient.Name.Should().Be(patientSearchedName);
            patient.Gender.Should().Be(patientGender);
            patient.YearOfBirth.Should().Be(patientYob);
        }

        [Fact]
        private async void PatientRepositoryWhereQuery_ReturnsAnHIPPatientWithSearchedNameInstedOfFoundName_WhenPatientFoundInOpenMrs()
        {
            const string patientSearchedName = "patient name";
            Gender? patientGender = Gender.F;

            ushort? patientYob = 1981;
            var patientEnquiry =
                new PatientEnquiry(
                    "id", verifiedIdentifiers: null, unverifiedIdentifiers: null,
                    patientSearchedName, patientGender, patientYob);
            var request = new DiscoveryRequest(patientEnquiry, "requestId", "transactionId", DateTime.Now);
            var repo = new OpenMrsPatientMatchingRepository(patientDal.Object);

            var patient = repo.Where(request).Result.Single();

            patient.Name.Should().Be(patientSearchedName);
        }
    }
}