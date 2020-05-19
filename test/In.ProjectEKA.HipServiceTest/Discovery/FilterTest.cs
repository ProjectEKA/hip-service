namespace In.ProjectEKA.HipServiceTest.Discovery
{
    using System.Linq;
    using Bogus;
    using Builder;
    using FluentAssertions;
    using HipLibrary.Patient.Model;
    using HipService.Discovery;
    using Xunit;
    using static Builder.TestBuilders;

    [Collection("Patient Filter Tests")]
    public class FilterTest
    {
        [Fact]
        private void ShouldFilterAndReturnAPatientByVerifiedIdentifierGenderAgeName()
        {
            var patientName = Faker().Name.FullName();
            const string patientPhoneNumber = "99999999999";
            var verifiedIdentifiers = Identifier()
                .GenerateLazy(10)
                .Select(builder => builder.Build())
                .Append(new IdentifierBuilder
                {
                    Type = IdentifierType.MOBILE,
                    Value = patientPhoneNumber
                }.Build());
            var unverifiedIdentifiers = Identifier()
                .GenerateLazy(10)
                .Select(builder => builder.Build());
            var yearOfBirth = (ushort) Faker().Date.Past().Year;
            var patientGender = Faker().PickRandom<Gender>();
            var discoveryRequest = new DiscoveryRequest(
                new PatientEnquiry(Faker().Random.Hash(),
                    verifiedIdentifiers,
                    unverifiedIdentifiers,
                    patientName,
                    patientGender,
                    yearOfBirth),
                Faker().Random.String(), RandomString(), RandomString());
            var patients = Patient()
                .GenerateLazy(10)
                .Append(Patient().Rules((_, patient) =>
                {
                    patient.PhoneNumber = patientPhoneNumber;
                    patient.YearOfBirth = yearOfBirth;
                    patient.Gender = patientGender;
                    patient.Name = patientName;
                }).Generate())
                .Append(Patient().Rules((_, patient) => { patient.PhoneNumber = patientPhoneNumber; }).Generate());

            var filteredPatients = Filter.Do(patients, discoveryRequest);

            filteredPatients.Count().Should().Be(1);
        }

        [Fact]
        private void ShouldFilterAndReturnMultiplePatientsByPhoneNumber()
        {
            const string mobileNumber = "99999999999";
            var verifiedIdentifiers = Identifier()
                .GenerateLazy(10)
                .Select(builder => builder.Build())
                .Append(new Identifier(IdentifierType.MOBILE, mobileNumber));
            var unverifiedIdentifiers = Identifier()
                .GenerateLazy(10)
                .Select(builder => builder.Build());
            var gender = Faker().PickRandom<Gender>();
            var dateOfBirth = (ushort) Faker().Date.Past().Year;
            var name = Faker().Name.FullName();
            var discoveryRequest = new DiscoveryRequest(
                new PatientEnquiry(new Faker().Random.Hash(),
                    verifiedIdentifiers,
                    unverifiedIdentifiers,
                    name,
                    gender,
                    dateOfBirth),
                Faker().Random.String(), RandomString(), RandomString());
            var patients = Patient()
                .GenerateLazy(10)
                .Append(Patient().Rules((_, patient) =>
                {
                    patient.PhoneNumber = mobileNumber;
                    patient.Gender = gender;
                    patient.Name = name;
                    patient.YearOfBirth = dateOfBirth;
                }).Generate())
                .Append(Patient().Rules((_, patient) =>
                {
                    patient.PhoneNumber = mobileNumber;
                    patient.Gender = gender;
                    patient.Name = name;
                    patient.YearOfBirth = dateOfBirth;
                }).Generate());

            var filteredPatients = Filter.Do(patients, discoveryRequest);

            filteredPatients.Count().Should().Be(2);
        }

        [Fact]
        private void ShouldFilterAndReturnAPatientWhenNameAndAgeAreNull()
        {
            const string mobileNumber = "99999999999";
            const Gender patientGender = Gender.F;
            var verifiedIdentifiers = Identifier()
                .GenerateLazy(10)
                .Select(builder => builder.Build())
                .Append(new Identifier(IdentifierType.MOBILE, mobileNumber));
            var unverifiedIdentifiers = Identifier()
                .GenerateLazy(10)
                .Select(builder => builder.Build());
            var discoveryRequest = new DiscoveryRequest(
                new PatientEnquiry(Faker().Random.Hash(),
                    verifiedIdentifiers,
                    unverifiedIdentifiers,
                    null,
                    patientGender,
                    null), Faker().Random.String(), RandomString(), RandomString());
            var patients = Patient()
                .GenerateLazy(10)
                .Append(Patient().Rules((_, patient) =>
                {
                    patient.PhoneNumber = mobileNumber;
                    patient.Gender = patientGender;
                }).Generate())
                .Append(Patient().Rules((_, patient) => { patient.PhoneNumber = mobileNumber; }).Generate());

            var filteredPatients = Filter.Do(patients, discoveryRequest);

            filteredPatients.Count().Should().Be(1);
        }
    }
}