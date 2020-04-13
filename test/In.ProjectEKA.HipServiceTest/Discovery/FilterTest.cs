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
            var filter = new Filter();
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
            var discoveryRequest = new DiscoveryRequest(
                new PatientEnquiry(Faker().Random.Hash(),
                    verifiedIdentifiers,
                    unverifiedIdentifiers,
                    patientName,
                    Faker().PickRandom<Gender>(),
                    Faker().Date.Past().Year), Faker().Random.String());
            var patients = Patient()
                .GenerateLazy(10)
                .Append(Patient().Rules((_, patient) =>
                {
                    patient.PhoneNumber = patientPhoneNumber;
                    patient.Name = patientName;
                }).Generate())
                .Append(Patient().Rules((_, patient) => { patient.PhoneNumber = patientPhoneNumber; }).Generate());

            var filteredPatients = filter.Do(patients, discoveryRequest);

            filteredPatients.Count().Should().Be(1);
        }

        [Fact]
        private void ShouldFilterAndReturnMultiplePatientsByPhoneNumber()
        {
            var filter = new Filter();
            const string mobileNumber = "99999999999";
            var verifiedIdentifiers = Identifier()
                .GenerateLazy(10)
                .Select(builder => builder.Build())
                .Append(new Identifier(IdentifierType.MOBILE, mobileNumber));
            var unverifiedIdentifiers = Identifier()
                .GenerateLazy(10)
                .Select(builder => builder.Build());
            var discoveryRequest = new DiscoveryRequest(
                new PatientEnquiry(new Faker().Random.Hash(), verifiedIdentifiers,
                    unverifiedIdentifiers,
                    null,
                    Faker().PickRandom<Gender>(),
                    Faker().Date.Past().Year), Faker().Random.String());
            var patients = Patient()
                .GenerateLazy(10)
                .Append(Patient().Rules((_, patient) => { patient.PhoneNumber = mobileNumber; }).Generate())
                .Append(Patient().Rules((_, patient) => { patient.PhoneNumber = mobileNumber; }).Generate());

            var filteredPatients = filter.Do(patients, discoveryRequest);

            filteredPatients.Count().Should().Be(2);
        }

        [Fact]
        private void ShouldFilterAndReturnMultiplePatientsByGender()
        {
            var filter = new Filter();
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
                    Faker().Date.Past().Year),
                Faker().Random.String());
            var patients = Patient()
                .GenerateLazy(10)
                .Append(Patient().Rules((_, patient) =>
                {
                    patient.PhoneNumber = mobileNumber;
                    patient.Gender = patientGender.ToString();
                }).Generate())
                .Append(Patient().Rules((_, patient) => { patient.PhoneNumber = mobileNumber; }).Generate());

            var filteredPatients = filter.Do(patients, discoveryRequest);

            filteredPatients.Count().Should().Be(1);
        }
    }
}