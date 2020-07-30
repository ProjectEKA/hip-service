namespace In.ProjectEKA.HipServiceTest.Discovery
{
    using System;
    using System.Linq;
    using Builder;
    using FluentAssertions;
    using HipLibrary.Patient.Model;
    using HipService.Discovery;
    using Xunit;
    using static Builder.TestBuilders;


    [Collection("Patient Filter Tests")]
    public class FilterTest
    {
        private class Linda
        {
            public const string Name = "Linda";
            public const Gender Sex = Gender.F;
            public const string PhoneNumber = "99999999999";
            public const ushort YearOfBirth = 1972;
        }
        private class John
        {
            public const string Name = "John";
            public const Gender Sex = Gender.M;
            public const string PhoneNumber = "11111111111";
            public const ushort YearOfBirth = 1994;
        }

        [Fact]
        private void ShouldFilterAndReturnAPatientByVerifiedIdentifierGenderAgeName()
        {
            var discoveryRequest = BuildDiscoveryRequest(Linda.Name, Linda.Sex, Linda.PhoneNumber, Linda.YearOfBirth);
            var patients = Patient()
                .GenerateLazy(10)
                .Append(BuildLinda())
                .Append(BuildPatient(null, Gender.M, Linda.PhoneNumber, 0));

            var filteredPatients = Filter.Do(patients, discoveryRequest);

            filteredPatients.Count().Should().Be(1);
        }


        [Theory]
        [InlineData(Linda.Name, Linda.Sex, Linda.PhoneNumber, Linda.YearOfBirth, true)]

        [InlineData(Linda.Name, null, null, 0, false)]
        [InlineData(null, Linda.Sex, Linda.PhoneNumber, Linda.YearOfBirth, false)]
        [InlineData(John.Name, Linda.Sex, Linda.PhoneNumber, Linda.YearOfBirth, false)]

        [InlineData(null, Linda.Sex, null, 0, false)]
        [InlineData(Linda.Name, John.Sex, Linda.PhoneNumber, Linda.YearOfBirth, false)]

        [InlineData(null, null, Linda.PhoneNumber, 0, false)]

        [InlineData(null, null, null, Linda.YearOfBirth, false)]
        [InlineData(Linda.Name, Linda.Sex, Linda.PhoneNumber, John.YearOfBirth, false)]
        private void ShouldFilterAndReturnAPatientWithPartialDataRecordByFullDataRequest(
            string recordedName, Gender? recordedGender, string recordedPhoneNumber, ushort recordedYearOfBirth, bool isExpectedToMatch)
        {
            var discoveryRequest = BuildDiscoveryRequest(Linda.Name, Linda.Sex, Linda.PhoneNumber, Linda.YearOfBirth);
            var patients = Patient().GenerateLazy(0).Append(BuildPatient(recordedName, recordedGender, recordedPhoneNumber, recordedYearOfBirth));

            var filteredPatients = Filter.Do(patients, discoveryRequest);

            if (isExpectedToMatch)
            {
                filteredPatients.Count().Should().Be(1);
            }
            else
            {
                filteredPatients.Count().Should().Be(0);
            }
        }

        [Theory]
        [InlineData(Linda.Name, Linda.Sex, Linda.PhoneNumber, Linda.YearOfBirth, true)]

        [InlineData(Linda.Name, null, null, null, false)]
        [InlineData(null, Linda.Sex, Linda.PhoneNumber, Linda.YearOfBirth, false)]
        [InlineData(John.Name, Linda.Sex, Linda.PhoneNumber, Linda.YearOfBirth, false)]

        [InlineData(null, Linda.Sex, null, null, false)]
        [InlineData(Linda.Name, John.Sex, Linda.PhoneNumber, Linda.YearOfBirth, false)]

        [InlineData(null, null, Linda.PhoneNumber, null, false)]

        [InlineData(null, null, null, Linda.YearOfBirth, false)]
        [InlineData(Linda.Name, Linda.Sex, Linda.PhoneNumber, John.YearOfBirth, false)]

        [InlineData(Linda.Name, null, Linda.PhoneNumber, Linda.YearOfBirth, true)]
        [InlineData(Linda.Name, Linda.Sex, Linda.PhoneNumber, null, true)]
        private void ShouldFilterAndReturnAPatientWithFullDataRecordByPartialDataRequest(
            string requestName, Gender? requestGender, string requestPhoneNumber, ushort? requestYearOfBirth, bool isExpectedToMatch)
        {
            var discoveryRequest = BuildDiscoveryRequest(requestName, requestGender, requestPhoneNumber, requestYearOfBirth);
            var patients = Patient().GenerateLazy(0).Append(BuildPatient(Linda.Name, Linda.Sex, Linda.PhoneNumber, Linda.YearOfBirth));

            var filteredPatients = Filter.Do(patients, discoveryRequest);

            if (isExpectedToMatch)
            {
                filteredPatients.Count().Should().Be(1);
            }
            else
            {
                filteredPatients.Count().Should().Be(0);
            }
        }

        [Fact]
        private void ShouldFilterAndReturnAPatientRegardlessThePhoneNumber()
        {
            var discoveryRequest = BuildDiscoveryRequest(Linda.Name, Linda.Sex, Linda.PhoneNumber, Linda.YearOfBirth);
            var patients = Patient().GenerateLazy(0).Append(BuildPatient(Linda.Name, Linda.Sex, John.PhoneNumber, Linda.YearOfBirth));

            var filteredPatients = Filter.Do(patients, discoveryRequest);

            filteredPatients.Count().Should().Be(1);
        }


        [Theory]
        [InlineData("linda", "Linda")]
        [InlineData("liNda", "Linda")]
        [InlineData("Linda", "Linda")]
        [InlineData("LINDA", "Linda")]
        [InlineData("Linda", "linda")]
        [InlineData("Linda", "linDa")]
        [InlineData("Linda", "LINDA")]
        [InlineData("UnicodeLïnda", "UNICODELÏNDA")]
        private void ShouldFilterAndReturnAPatientRegardlessTheCasingOfName(string requestName, string recordedName)
        {
            var discoveryRequest = BuildDiscoveryRequest(requestName, Linda.Sex, Linda.PhoneNumber, Linda.YearOfBirth);
            var patients = Patient().GenerateLazy(0).Append(BuildPatient(recordedName, Linda.Sex, John.PhoneNumber, Linda.YearOfBirth));

            var filteredPatients = Filter.Do(patients, discoveryRequest);

            filteredPatients.Count().Should().Be(1);
        }

        [Fact]
        private void ShouldFilterAndNotReturnAPatientByFuzzyName()
        {
            var discoveryRequest = BuildDiscoveryRequest(Linda.Name, Linda.Sex, Linda.PhoneNumber, Linda.YearOfBirth);
            var patients = Patient()
                .GenerateLazy(10)
                .Append(BuildPatient("Lunda", Linda.Sex, Linda.PhoneNumber, Linda.YearOfBirth))
                .Append(BuildPatient(null, Gender.M, Linda.PhoneNumber, 0));

            var filteredPatients = Filter.Do(patients, discoveryRequest);

            filteredPatients.Count().Should().Be(0);
        }

        [Fact]
        private void ShouldFilterAndReturnMultiplePatientsByPhoneNumber()
        {
            var discoveryRequest = BuildDiscoveryRequest(Linda.Name, Linda.Sex, Linda.PhoneNumber, Linda.YearOfBirth);
            var patients = Patient()
                .GenerateLazy(10)
                .Append(BuildLinda())
                .Append(BuildLinda());

            var filteredPatients = Filter.Do(patients, discoveryRequest);

            filteredPatients.Count().Should().Be(2);
        }

        [Fact]
        private void ShouldFilterAndReturnAPatientWhenNameAndAgeAreNull()
        {
            var discoveryRequest = BuildDiscoveryRequest(null, Linda.Sex, Linda.PhoneNumber, null);
            var patients = Patient()
                .GenerateLazy(10)
                .Append(BuildPatient(null, Linda.Sex, Linda.PhoneNumber, 0))
                .Append(BuildPatient(null, Gender.M, Linda.PhoneNumber, 0));

            var filteredPatients = Filter.Do(patients, discoveryRequest);

            filteredPatients.Count().Should().Be(1);
        }

        private DiscoveryRequest BuildDiscoveryRequest(string name, Gender? gender, string phoneNumber, ushort? yearOfBirth = null)
        {
            var verifiedIdentifiers = Identifier()
                   .GenerateLazy(0)
                   .Select(builder => builder.Build())
                   .Append(new IdentifierBuilder
                   {
                       Type = IdentifierType.MOBILE,
                       Value = phoneNumber
                   }.Build());
            var unverifiedIdentifiers = Identifier()
                .GenerateLazy(0)
                .Select(builder => builder.Build())
                .Append(new IdentifierBuilder
                {
                    Type = IdentifierType.MOBILE,
                    Value = phoneNumber
                }.Build());
            return new DiscoveryRequest(
                new PatientEnquiry(Faker().Random.Hash(),
                    verifiedIdentifiers,
                    unverifiedIdentifiers,
                    name,
                    gender,
                    yearOfBirth),
                Faker().Random.String(),
                RandomString(),
                DateTime.Now);
        }

        private Patient BuildLinda()
        {
            return BuildPatient(Linda.Name, Linda.Sex, Linda.PhoneNumber, Linda.YearOfBirth);
        }

        private Patient BuildPatient(string name, Gender? gender, string phoneNumber, ushort? yearOfBirth)
        {
            return Patient().Rules((_, patient) =>
            {
                patient.PhoneNumber = phoneNumber;
                patient.YearOfBirth = yearOfBirth;
                patient.Gender = gender;
                patient.Name = name;
            }).Generate();
        }
    }
}