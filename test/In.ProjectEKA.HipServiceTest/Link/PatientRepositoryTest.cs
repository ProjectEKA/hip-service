using System.Collections.Generic;
using System.Threading.Tasks;
using In.ProjectEKA.HipService.OpenMrs;
using Moq;
using Xunit;
using OpenMrsPatient = Hl7.Fhir.Model.Patient;
using OpenMrsPatientName = Hl7.Fhir.Model.HumanName;
using OpenMrsGender = Hl7.Fhir.Model.AdministrativeGender;
using FluentAssertions;
using In.ProjectEKA.HipLibrary.Patient.Model;
using In.ProjectEKA.HipLibrary.Patient;
using System.Linq;

namespace In.ProjectEKA.HipServiceTest.Link
{
    [Collection("Patient Repository Tests")]
    public class PatientRepositoryTest
    {
        private Mock<IPatientDal> patientDal = new Mock<IPatientDal>();
        private Mock<ICareContextRepository> careContextRepository = new Mock<ICareContextRepository>();
        private Mock<IPhoneNumberRepository> phoneNumberRepository = new Mock<IPhoneNumberRepository>();

        public PatientRepositoryTest()
        {
            var careContextRepresentations = new[]
            {
                new CareContextRepresentation("testReferrenceNumber1", "display1"),
                new CareContextRepresentation("testReferenceNumber2", "display2")
            };
            var phoneNumber = "+91-9999999999";
            patientDal.Setup(e => e.LoadPatientAsyncWithIndentifier(It.IsAny<string>()))
                .Returns(Task.FromResult(
                    new OpenMrsPatient() {
                        Name = new List<OpenMrsPatientName>{
                            new OpenMrsPatientName {GivenElement = new List<Hl7.Fhir.Model.FhirString>{
                                new Hl7.Fhir.Model.FhirString("test")
                            }}
                        },
                        Gender = OpenMrsGender.Female,
                        BirthDate = "1981",
                        Id = "someid"
                    }
                ));
            careContextRepository.Setup(e => e.GetCareContexts(It.IsAny<string>())).Returns(Task.FromResult(
                new List<CareContextRepresentation>(careContextRepresentations).AsEnumerable()
            ));
            phoneNumberRepository.Setup(e => e.GetPhoneNumber(It.IsAny<string>())).ReturnsAsync(phoneNumber);
        }

        [Fact]
        private async void PatientRepositoryPatientWith_ReturnHIPPatient()
        {
            var patientIdentifier = "someidentifier";
            var repo = new OpenMrsPatientRepository(patientDal.Object, careContextRepository.Object, phoneNumberRepository.Object);

            var patient = await repo.PatientWithAsync(patientIdentifier);

            var patientValue = patient.ValueOr(new Patient());
            patientDal.Verify( x => x.LoadPatientAsyncWithIndentifier(patientIdentifier), Times.Once);
            careContextRepository.Verify(x => x.GetCareContexts(patientValue.Uuid), Times.Once);
            patientValue.Name.Should().Be("test");
            patientValue.Gender.Should().Be(Gender.F);
            patientValue.YearOfBirth.Should().Be(1981);
            patientValue.CareContexts.First().ReferenceNumber.Should().Be("testReferrenceNumber1");
            patientValue.CareContexts.Count().Should().Be(2);
            patientValue.PhoneNumber.Should().Be("+91-9999999999");
        }
    }
}