namespace In.ProjectEKA.HipServiceTest.Discovery.Patient.Ranker
{
    using FluentAssertions;
    using HipLibrary.Patient.Model.Response;
    using HipService.Discovery.Patient.Ranker;
    using Xunit;
    using Patient = HipService.Discovery.Patient.Model.Patient;

    [Collection("Fuzzy Match Names Tests")]
    public class FistNameRankerTest
    {
        [Fact]
        private void ShouldHaveHighestScore()
        {
            var patient = new Patient();
            patient.FirstName = "zack  ";
            var patientWithRank = new FirstNameRanker().Rank(patient, "Zack");
            patientWithRank.Meta.Should().BeEquivalentTo(MetaBuilder.FullMatchMeta(Match.FIRST_NAME));
            patientWithRank.Rank.Score.Should().Be(10);
        }

        [Fact]
        private void ShouldHaveHighScore()
        {
            var patient = new Patient();
            patient.FirstName = "Jack";
            var patientWithRank = new FirstNameRanker().Rank(patient, "Zack");
            patientWithRank.Meta.Should().BeEquivalentTo(MetaBuilder.FullMatchMeta(Match.FIRST_NAME));
            patientWithRank.Rank.Score.Should().Be(8);
            
        }

        [Fact]
        private void ShouldHavePoorScore()
        {
            var patient = new Patient();
            patient.FirstName = "Jackie";
            var patientWithRank = new FirstNameRanker().Rank(patient, "Zackey");
            patientWithRank.Meta.Should().BeEquivalentTo(MetaBuilder.FullMatchMeta(Match.EMPTY));
            patientWithRank.Rank.Score.Should().Be(0);
            
        }
    }
}