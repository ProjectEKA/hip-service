namespace In.ProjectEKA.HipServiceTest.Discovery
{
    using System;
    using System.Linq;
    using Builder;
    using FluentAssertions;
    using HipLibrary.Patient.Model;
    using HipService.Discovery;
    using Xunit;
    using Xunit.Sdk;
    using static Builder.TestBuilders;

    [Collection("Patient Filter Tests")]
    public class FilterTest
    {
        Patient Linda = new Patient
        {
            Name = "Linda",
            Gender = Gender.F,
            PhoneNumber = "99999999999",
            YearOfBirth = 1972
        };

        [Fact]
        private void ShouldFilterAndReturnAPatientByVerifiedIdentifierGenderAgeName()
        {
            var discoveryRequest = BuildDiscoveryRequest(Linda.Identifier, Linda.Name, Linda.Gender, Linda.PhoneNumber, Linda.YearOfBirth);
            var patients = Patient()
                .GenerateLazy(10)
                .Append(BuildLinda())
                .Append(BuildPatient(null, Gender.M, Linda.PhoneNumber, 0));

            var filteredPatients = Filter.Do(patients, discoveryRequest);

            filteredPatients.Count().Should().Be(1);
        }

        [Fact]
        private void ShouldFilterAndReturnMultiplePatientsByPhoneNumber()
        {
            var discoveryRequest = BuildDiscoveryRequest(Linda.Identifier, Linda.Name, Linda.Gender, Linda.PhoneNumber, Linda.YearOfBirth);
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
            var discoveryRequest = BuildDiscoveryRequest(Linda.Identifier, null, Linda.Gender, Linda.PhoneNumber, null);
            var patients = Patient()
                .GenerateLazy(10)
                .Append(BuildPatient(null, Linda.Gender, Linda.PhoneNumber, 0))
                .Append(BuildPatient(null, Gender.M, Linda.PhoneNumber, 0));

            var filteredPatients = Filter.Do(patients, discoveryRequest);

            filteredPatients.Count().Should().Be(1);
        }

        private DiscoveryRequest BuildDiscoveryRequest(string identifier, string name, Gender gender, string phoneNumber, ushort? yearOfBirth = null)
        {
            var verifiedIdentifiers = Identifier()
                   .GenerateLazy(10)
                   .Select(builder => builder.Build())
                   .Append(new IdentifierBuilder
                   {
                       Type = IdentifierType.MOBILE,
                       Value = phoneNumber
                   }.Build());
            var unverifiedIdentifiers = Identifier()
                .GenerateLazy(10)
                .Select(builder => builder.Build());
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