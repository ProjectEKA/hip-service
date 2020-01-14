using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using hip_service.Link.Patient;
using hip_service.OTP;
using hip_service_test.Link.Builder;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Xunit;

namespace hip_service_test.Link.Patient
{
    [Collection("Patient Verification Tests")]
    public class PatientVerificationTest
    {
        private readonly PatientVerification patientVerification;
        public PatientVerificationTest()
        {
            IConfiguration configuration = TestBuilder.GetIConfigurationRoot();
            patientVerification = new PatientVerification(configuration);
        }

        [Fact]
        private async void ReturnFailureOnOtpCreation()
        {
            var session = new Session(TestBuilder.Faker().Random.Hash()
                , new Communication(CommunicationMode.MOBILE,"+91666666666666"));
            
            var result = await patientVerification.SendTokenFor(session);
            
            result.Should().NotBeNull();
        }
    }
}