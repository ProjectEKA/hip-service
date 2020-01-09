using FluentAssertions;
using hip_service.Discovery.Patient.Helpers;
using hip_service.Discovery.Patient.Ranker;
using HipLibrary.Patient.Model;
using HipLibrary.Patient.Model.Response;
using Xunit;

namespace hip_service_test.Discovery.Patient.Ranker
{
    [Collection("Fuzzy Match Names Tests")]
    public class FistNameRankerTest
    {
        [Fact]
        private void shouldHaveHighestScore()
        {
            var patient = new hip_service.Discovery.Patient.Model.Patient();
            patient.FirstName = "zack  ";
            var patientWithRank = new FirstNameRanker().Rank(patient, "Zack");
            patientWithRank.Meta.Should().BeEquivalentTo(MetaBuilder.FullMatchMeta(Match.FIRST_NAME));
            patientWithRank.Rank.Score.Should().Be(10);
        }

        [Fact]
        private void shouldHaveHighScore()
        {
            var patient = new hip_service.Discovery.Patient.Model.Patient();
            patient.FirstName = "Jack";
            var patientWithRank = new FirstNameRanker().Rank(patient, "Zack");
            patientWithRank.Meta.Should().BeEquivalentTo(MetaBuilder.FullMatchMeta(Match.FIRST_NAME));
            patientWithRank.Rank.Score.Should().Be(8);
            
        }

        [Fact]
        private void shouldHavePoorScore()
        {
            var patient = new hip_service.Discovery.Patient.Model.Patient();
            patient.FirstName = "Jackie";
            var patientWithRank = new FirstNameRanker().Rank(patient, "Zackey");
            patientWithRank.Meta.Should().BeEquivalentTo(MetaBuilder.FullMatchMeta(Match.EMPTY));
            patientWithRank.Rank.Score.Should().Be(0);
            
        }
    }

           
}