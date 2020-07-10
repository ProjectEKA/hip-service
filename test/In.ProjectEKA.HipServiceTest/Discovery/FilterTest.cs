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
            public const string Gender2 = "F";
            public const string PhoneNumber = "99999999999";
            public const ushort YearOfBirth = 1972;

            public static Gender Gender { get { return HipLibrary.Patient.Model.Gender.F; } } 
        }
        private class John
        {
            public const string Name = "John";
            public const string Gender2 = "M";
            public const string PhoneNumber = "11111111111";
            public const ushort YearOfBirth = 1994;

            public static Gender Gender { get { return HipLibrary.Patient.Model.Gender.M; } }
        }

        [Fact]
        private void ShouldFilterAndReturnAPatientByVerifiedIdentifierGenderAgeName()
        {
            var discoveryRequest = BuildDiscoveryRequest(Linda.Name, Linda.Gender, Linda.PhoneNumber, Linda.YearOfBirth);
            var patients = Patient()
                .GenerateLazy(10)
                .Append(BuildLinda())
                .Append(BuildPatient(null, Gender.M, Linda.PhoneNumber, 0));

            var filteredPatients = Filter.Do(patients, discoveryRequest);

            filteredPatients.Count().Should().Be(1);
        }


        [Theory]
        [InlineData(Linda.Name, Linda.Gender2, Linda.PhoneNumber, Linda.YearOfBirth, true)]

        [InlineData(Linda.Name, null, null, 0, false)]
        [InlineData(null, Linda.Gender2, Linda.PhoneNumber, Linda.YearOfBirth, false)]
        [InlineData(John.Name, Linda.Gender2, Linda.PhoneNumber, Linda.YearOfBirth, false)]

        [InlineData(null, Linda.Gender2, null, 0, false)]
        [InlineData(Linda.Name, John.Gender2, Linda.PhoneNumber, Linda.YearOfBirth, false)]

        [InlineData(null, null, Linda.PhoneNumber, 0, false)]

        [InlineData(null, null, null, Linda.YearOfBirth, false)]
        [InlineData(Linda.Name, Linda.Gender2, Linda.PhoneNumber, John.YearOfBirth, false)]
        private void ShouldFilterAndReturnAPatientByVerifiedIdentifierGenderAgeName2(
            string recordedName, string recordedGenderText, string recordedPhoneNumber, ushort recordedYearOfBirth, bool isExpectedToMatch)
        {
            var discoveryRequest = BuildDiscoveryRequest(Linda.Name, Linda.Gender, Linda.PhoneNumber, Linda.YearOfBirth);
            var recordedGender = recordedGenderText == "F" ? Gender.F : Gender.M;
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

        [Fact]
        private void ShouldFilterAndReturnAPatientRegardlessThePhoneNumber()
        {
            var discoveryRequest = BuildDiscoveryRequest(Linda.Name, Linda.Gender, Linda.PhoneNumber, Linda.YearOfBirth);
            var patients = Patient().GenerateLazy(0).Append(BuildPatient(Linda.Name, Linda.Gender, John.PhoneNumber, Linda.YearOfBirth));

            var filteredPatients = Filter.Do(patients, discoveryRequest);

            filteredPatients.Count().Should().Be(1);
        }


        [Fact]
        private void ShouldFilterAndNotReturnAPatientByFuzzyName()
        {
            var discoveryRequest = BuildDiscoveryRequest(Linda.Name, Linda.Gender, Linda.PhoneNumber, Linda.YearOfBirth);
            var patients = Patient()
                .GenerateLazy(10)
                .Append(BuildPatient("Lunda", Linda.Gender, Linda.PhoneNumber, Linda.YearOfBirth))
                .Append(BuildPatient(null, Gender.M, Linda.PhoneNumber, 0));

            var filteredPatients = Filter.Do(patients, discoveryRequest);

            filteredPatients.Count().Should().Be(0);
        }

        [Fact]
        private void ShouldFilterAndReturnMultiplePatientsByPhoneNumber()
        {
            var discoveryRequest = BuildDiscoveryRequest(Linda.Name, Linda.Gender, Linda.PhoneNumber, Linda.YearOfBirth);
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
            var discoveryRequest = BuildDiscoveryRequest(null, Linda.Gender, Linda.PhoneNumber, null);
            var patients = Patient()
                .GenerateLazy(10)
                .Append(BuildPatient(null, Linda.Gender, Linda.PhoneNumber, 0))
                .Append(BuildPatient(null, Gender.M, Linda.PhoneNumber, 0));

            var filteredPatients = Filter.Do(patients, discoveryRequest);

            filteredPatients.Count().Should().Be(1);
        }

        private DiscoveryRequest BuildDiscoveryRequest(string name, Gender gender, string phoneNumber, ushort? yearOfBirth = null)
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
            return BuildPatient(Linda.Name, Linda.Gender, Linda.PhoneNumber, Linda.YearOfBirth);
        }

        private Patient BuildPatient(string name, Gender gender, string phoneNumber, ushort yearOfBirth)
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