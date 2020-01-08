using System.Linq;
using FluentAssertions;
using hip_service.Discovery.Patient;
using HipLibrary.Patient.Model;
using HipLibrary.Patient.Model.Request;
using Xunit;
using static hip_service_test.Discovery.Builder.TestBuilders;
using Bogus;

namespace hip_service_test.Discovery.Patient
{
    using Patient = HipLibrary.Patient.Model.Request.Patient;

    [Collection("Patient Filter Tests")]
    public class FilterTest
    {
        [Fact]
        private void ShouldFilterAndReturnAPatientByUnverifiedIdentifier()
        {
            var filter = new Filter();
            var patientFirstName = Faker().Name.FirstName();
            const string patientPhoneNumber = "99999999999";
            var verifiedIdentifiers = identifier()
                .GenerateLazy(10)
                .Select(builder => builder.build())
                .Append(new IdentifierBuilder
                {
                    Type = IdentifierType.MOBILE,
                    Value = patientPhoneNumber
                }.build());
            var unverifiedIdentifiers = identifier()
                .GenerateLazy(10)
                .Select(builder => builder.build());
            var discoveryRequest = new DiscoveryRequest(
                new Patient(Faker().Random.Hash(),
                    verifiedIdentifiers,
                    unverifiedIdentifiers,
                    patientFirstName,
                    Faker().Name.FirstName(),
                    Faker().PickRandom<Gender>(),
                    Faker().Date.Past()), Faker().Random.String());
            var patients = patient()
                .GenerateLazy(10)
                .Append(patient().Rules((_, patient) =>
                {
                    patient.PhoneNumber = patientPhoneNumber;
                    patient.FirstName = patientFirstName;
                }).Generate())
                .Append(patient().Rules((_, patient) => { patient.PhoneNumber = patientPhoneNumber; }).Generate());

            var filteredPatients = filter.Do(patients, discoveryRequest);

            filteredPatients.Count().Should().Be(1);
        }

        [Fact]
        private void ShouldFilterAndReturnMultiplePatientsByPhoneNumber()
        {
            var filter = new Filter();
            const string mobileNumber = "99999999999";
            var verifiedIdentifiers = identifier()
                .GenerateLazy(10)
                .Select(builder => builder.build())
                .Append(new Identifier(IdentifierType.MOBILE, mobileNumber));
            var unverifiedIdentifiers = identifier()
                .GenerateLazy(10)
                .Select(builder => builder.build());
            var discoveryRequest = new DiscoveryRequest(
                new Patient(new Faker().Random.Hash(), verifiedIdentifiers,
                    unverifiedIdentifiers,
                    null,
                    null,
                    Faker().PickRandom<Gender>(),
                    Faker().Date.Past()), Faker().Random.String());
            var patients = patient()
                .GenerateLazy(10)
                .Append(patient().Rules((_, patient) => { patient.PhoneNumber = mobileNumber; }).Generate())
                .Append(patient().Rules((_, patient) => { patient.PhoneNumber = mobileNumber; }).Generate());

            var filteredPatients = filter.Do(patients, discoveryRequest);

            filteredPatients.Count().Should().Be(2);
        }

        [Fact]
        private void ShouldFilterAndReturnMultiplePatientsByGender()
        {
            var filter = new Filter();
            const string mobileNumber = "99999999999";
            const Gender patientGender = Gender.F;
            var verifiedIdentifiers = identifier()
                .GenerateLazy(10)
                .Select(builder => builder.build())
                .Append(new Identifier(IdentifierType.MOBILE, mobileNumber));
            var unverifiedIdentifiers = identifier()
                .GenerateLazy(10)
                .Select(builder => builder.build());
            var discoveryRequest = new DiscoveryRequest(
                new Patient(Faker().Random.Hash(),
                    verifiedIdentifiers,
                    unverifiedIdentifiers,
                    null,
                    null,
                    patientGender,
                    Faker().Date.Past()), Faker().Random.String());
            var patients = patient()
                .GenerateLazy(10)
                .Append(patient().Rules((_, patient) =>
                {
                    patient.PhoneNumber = mobileNumber;
                    patient.Gender = patientGender.ToString();
                }).Generate())
                .Append(patient().Rules((_, patient) => { patient.PhoneNumber = mobileNumber; }).Generate());

            var filteredPatients = filter.Do(patients, discoveryRequest);

            filteredPatients.Count().Should().Be(1);
        }
    }
}