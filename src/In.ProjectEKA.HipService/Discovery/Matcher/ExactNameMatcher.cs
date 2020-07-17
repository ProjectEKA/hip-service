namespace In.ProjectEKA.HipService.Discovery.Matcher
{
    using System;

    public static class ExactNameMatcher
    {
        public static bool IsMatch(string left, string right)
        {
            left ??= "";
            right ??= "";
            return left.Equals(right, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}