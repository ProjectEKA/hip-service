namespace In.ProjectEKA.HipServiceTest.Discovery.Helpers
{
    using FluentAssertions;
    using HipService.Discovery.Matcher;
    using Xunit;
    using static Builder.TestBuilders;

    [Collection("Fuzzy Match Names Tests")]
    public class FuzzyNameMatcherTest
    {
        [Fact]
        private void ShouldMatchExactly()
        {
            var name = Faker().Name.FullName();
            FuzzyNameMatcher.LevenshteinDistance(name, name).Should().Be(0);
        }

        [Fact]
        private void ShouldHaveOneDifference()
        {
            FuzzyNameMatcher.LevenshteinDistance("Ajoy", "Ajay").Should().Be(1);
            FuzzyNameMatcher.LevenshteinDistance("Chethan", "Chetan").Should().Be(1);
            FuzzyNameMatcher.LevenshteinDistance("github", "git hub").Should().Be(1);
        }

        [Fact]
        private void ShouldHaveTwoDifferences()
        {
            FuzzyNameMatcher.LevenshteinDistance("Akshatha", "Aksata").Should().Be(2);
            FuzzyNameMatcher.LevenshteinDistance("Aksata", "Akshatha").Should().Be(2);
        }

        [Fact]
        private void ShouldHaveMoreThanTwoDifference()
        {
            FuzzyNameMatcher.LevenshteinDistance("Aatha", "Aksata").Should().BeGreaterThan(2);
            FuzzyNameMatcher.LevenshteinDistance("Mama", "Nina").Should().BeGreaterThan(2);
            FuzzyNameMatcher.LevenshteinDistance(Faker().Random.String(), Faker().Random.String())
                .Should().BeGreaterThan(2);
        }
    }
}