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
        private void ShouldFilterAndReturnAPatientByUnverifiedIdentifier()
        {
            var patientFirstName = Faker().Name.FirstName();
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
            var dateOfBirth = Faker().Date.Past();
            var patientGender = Faker().PickRandom<Gender>();
            var discoveryRequest = new DiscoveryRequest(
                new PatientEnquiry(Faker().Random.Hash(),
                    verifiedIdentifiers,
                    unverifiedIdentifiers,
                    patientFirstName,
                    Faker().Name.FirstName(),
                    patientGender,
                    dateOfBirth), Faker().Random.String());
            var patients = Patient()
                .GenerateLazy(10)
                .Append(Patient().Rules((_, patient) =>
                {
                    patient.PhoneNumber = patientPhoneNumber;
                    patient.FirstName = patientFirstName;
                    patient.YearOfBirth = (ushort) dateOfBirth.Year;
                    patient.Gender = patientGender;
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
            var dateOfBirth = Faker().Date.Past();
            var name = Faker().Name.FullName();
            var discoveryRequest = new DiscoveryRequest(
                new PatientEnquiry(new Faker().Random.Hash(), verifiedIdentifiers,
                    unverifiedIdentifiers,
                    name,
                    null,
                    gender,
                    dateOfBirth), Faker().Random.String());
            var patients = Patient()
                .GenerateLazy(10)
                .Append(Patient().Rules((_, patient) =>
                {
                    patient.PhoneNumber = mobileNumber;
                    patient.Gender = gender;
                    patient.FirstName = name;
                    patient.YearOfBirth = (ushort) dateOfBirth.Year;
                }).Generate())
                .Append(Patient().Rules((_, patient) =>
                {
                    patient.PhoneNumber = mobileNumber;
                    patient.Gender = gender;
                    patient.FirstName = name;
                    patient.YearOfBirth = (ushort) dateOfBirth.Year;
                }).Generate());

            var filteredPatients = Filter.Do(patients, discoveryRequest);

            filteredPatients.Count().Should().Be(2);
        }

        [Fact]
        private void ShouldFilterAndReturnMultiplePatientsByGender()
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
                    null,
                    patientGender,
                    null), Faker().Random.String());
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