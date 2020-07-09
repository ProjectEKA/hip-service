namespace In.ProjectEKA.HipService.Discovery.Matcher
{
    using System;

    public static class ExactNameMatcher
    {
        public static bool IsMatch(string s1, string s2)
        {
            var string1 = s1 == null ? "" : s1;
            var string2 = s2 == null ? "" : s2;
            return string1.Equals(string2, StringComparison.InvariantCulture);
        }
    }
}