namespace In.ProjectEKA.HipServiceTest.Discovery
{
    using System;
    using FluentAssertions;
    using HipLibrary.Patient.Model;
    using Xunit;
    using System.Collections.Generic;
    using System.Linq;
    using OpenMrsPatient = Hl7.Fhir.Model.Patient;
    using OpenMrsPatientName = Hl7.Fhir.Model.HumanName;
    using OpenMrsGender = Hl7.Fhir.Model.AdministrativeGender;
    using OpenMrsIdentifier = Hl7.Fhir.Model.Identifier;
    using In.ProjectEKA.HipService.OpenMrs.Mappings;

    public class PatientExtensionsTest
    {
        const string patientName = "Patient name";

        [Fact]
        private void ToHipPatient_GivenOpenMrsPatientWithMultipleNames_UsesSearchedName()
        {
            var openMrsPatient = new OpenMrsPatient() {
                Name = new List<OpenMrsPatientName>{  new OpenMrsPatientName() { Text = $"OpenMRS {patientName}" }, new OpenMrsPatientName() { Text = "a second name" } },
                Gender = OpenMrsGender.Female,
                BirthDate = "1981"
            };

            var hipPatient = openMrsPatient.ToHipPatient(patientName);

            openMrsPatient.Name.Count().Should().Be(2);
            hipPatient.Name.Should().Be(patientName);
        }

        [Theory]
        [InlineData(OpenMrsGender.Male, Gender.M)]
        [InlineData(OpenMrsGender.Female, Gender.F)]
        [InlineData(OpenMrsGender.Other, Gender.O)]
        [InlineData(null, null)]
        private void ToHipPatient_GivenOpenMrsPatient_GenderIsMappedCorrectly(OpenMrsGender? sourceOpenMrsGender, Gender? expectedHipGender)
        {

            var openMrsPatient = new OpenMrsPatient() {
                Name = new List<OpenMrsPatientName>{  new OpenMrsPatientName() { Text = patientName } },
                Gender = sourceOpenMrsGender,
                BirthDate = "1981"
            };

            var hipPatient = openMrsPatient.ToHipPatient(patientName);
            
            hipPatient.Gender.Should().Be(expectedHipGender);

        }


        [Theory]
        [InlineData("1981", (UInt16)1981)]
        [InlineData("1973-06", (UInt16)1973)]
        [InlineData("1905-08-23", (UInt16)1905)]
        [InlineData(null, null)]
        private void ToHipPatient_GivenOpenMrsPatient_YearOfBirthIsCalculatedFromBirthDate(string sourceBirthDate, ushort? expectedYearOfBirth)
        {
            // The hl7 date format is YYYY, YYYY-MM, or YYYY-MM-DD, e.g. 2018, 1973-06, or 1905-08-23. 
            // https://www.hl7.org/fhir/datatypes.html#date

            var openMrsPatient = new OpenMrsPatient() {
                Name = new List<OpenMrsPatientName>{  new OpenMrsPatientName() { Text = patientName } },
                Gender = OpenMrsGender.Female,
                BirthDate = sourceBirthDate
            };

            var hipPatient = openMrsPatient.ToHipPatient(patientName);

            hipPatient.YearOfBirth.Should().Be(expectedYearOfBirth);
        }

        [Theory]
        [InlineData(Gender.M, OpenMrsGender.Male)]
        [InlineData(Gender.F, OpenMrsGender.Female)]
        [InlineData(Gender.O, OpenMrsGender.Other)]
        [InlineData(null, null)]
        private void ToOpenMrsGender_GivenHipGender_ConvertsToOpenMrsGender(Gender? hipGender, OpenMrsGender? openMrsGender)
        {
            hipGender.ToOpenMrsGender().Should().Be(openMrsGender);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("BAH203001")]
        [InlineData(null)]
        private void ToHipPatient_GivenOpenMrsPatient_IdentifierIsMappedToPatientIdentifier(string identifier)
        {
            var openMrsPatient = new OpenMrsPatient()
            {
                Name = new List<OpenMrsPatientName> { new OpenMrsPatientName() { Text = patientName } },
                Gender = OpenMrsGender.Female,
                BirthDate = "1999",
                Identifier = new List<OpenMrsIdentifier> {
                    new OpenMrsIdentifier() {
                        System = "Patient Identifier",
                        Value = identifier,
                    },
                    new OpenMrsIdentifier() {
                        System = "National ID",
                        Value = "Some value else that we don't need",
                    }
                }
            };

            var hipPatient = openMrsPatient.ToHipPatient(patientName);

            hipPatient.Identifier.Should().Be(identifier);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("12345678-1234-1234-1234-123456789abc")]
        [InlineData(null)]
        private void ToHipPatient_GivenOpenMrsPatient_UuidIsMappedToPatientUuid(string patientUuid)
        {
            var openMrsPatient = new OpenMrsPatient()
            {
                Name = new List<OpenMrsPatientName> { new OpenMrsPatientName() { Text = patientName } },
                Gender = OpenMrsGender.Female,
                BirthDate = "1999",
                Id = patientUuid,
            };

            var hipPatient = openMrsPatient.ToHipPatient(patientName);

            hipPatient.Uuid.Should().Be(patientUuid);
        }
    }
}