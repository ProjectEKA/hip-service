using FluentAssertions;
using hip_service.Discovery.Patient.Helpers;
using Xunit;

namespace hip_service_test.Discovery.Patient
{
    [Collection("Fuzzy Match Names Tests")]
    public class FuzzyNameMatcherTest
    {
        [Fact]
        private void shouldHaveOneDifference()
        {
            FuzzyNameMatcher.LevenshteinDistance("Ajoy", "Ajay").Should().Be(1);
            FuzzyNameMatcher.LevenshteinDistance("Chethan", "Chetan").Should().Be(1);            
            FuzzyNameMatcher.LevenshteinDistance("github", "git hub").Should().Be(1); 
        }

        [Fact]
        private void shouldHaveTwoDifferences()
        {
            FuzzyNameMatcher.LevenshteinDistance("Akshatha", "Aksata").Should().Be(2); 
            FuzzyNameMatcher.LevenshteinDistance("Aksata", "Akshatha").Should().Be(2); 
            
        }
    }

           
}