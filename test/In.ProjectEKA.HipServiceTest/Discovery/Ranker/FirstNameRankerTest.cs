namespace In.ProjectEKA.HipServiceTest.Discovery.Ranker
{
    using FluentAssertions;
    using HipLibrary.Patient.Model;
    using HipService.Discovery.Ranker;
    using Xunit;
    using static HipService.Discovery.Ranker.MetaBuilder;

    [Collection("Fuzzy Match Names Tests")]
    public class FistNameRankerTest
    {
        [Fact]
        private void ShouldHaveHighestScore()
        {
            var patient = new HipLibrary.Patient.Model.Patient {FirstName = "zack  "};
            var patientWithRank = new FirstNameRanker().Rank(patient, "Zack");
            patientWithRank.Meta.Should().BeEquivalentTo(FullMatchMeta(Match.FirstName));
            patientWithRank.Rank.Score.Should().Be(10);
        }

        [Fact]
        private void ShouldHaveHighScore()
        {
            var patient = new HipLibrary.Patient.Model.Patient {FirstName = "Jack"};
            var patientWithRank = new FirstNameRanker().Rank(patient, "Zack");
            patientWithRank.Meta.Should().BeEquivalentTo(FullMatchMeta(Match.FirstName));
            patientWithRank.Rank.Score.Should().Be(8);
            
        }

        [Fact]
        private void ShouldHavePoorScore()
        {
            var patient = new HipLibrary.Patient.Model.Patient {FirstName = "Jackie"};
            var patientWithRank = new FirstNameRanker().Rank(patient, "Zackey");
            patientWithRank.Meta.Should().BeEquivalentTo(FullMatchMeta(Match.Empty));
            patientWithRank.Rank.Score.Should().Be(0);
        }
    }
}