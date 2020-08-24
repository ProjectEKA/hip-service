namespace In.ProjectEKA.HipService.Discovery.Matcher
{
    using System;

    public static class FuzzyNameMatcher
    {
        public static int LevenshteinDistance(string s1, string s2)
        {
            var string1 = s1 == null ? "" : s1.Trim().ToLowerInvariant();
            var string2 = s2 == null ? "" : s2.Trim().ToLowerInvariant();
            var l1 = string1.Length;
            var l2 = string2.Length;
            var d = new int[l1 + 1, l2 + 1];

            if (l1 == 0) return l2;

            if (l2 == 0) return l1;

            for (var i = 0; i <= l1; i++) d[i, 0] = i;
            for (var j = 0; j <= l2; j++) d[0, j] = j;

            for (var j = 1; j <= l2; j++)
                for (var i = 1; i <= l1; i++)
                    if (string1[i - 1] == string2[j - 1])
                        d[i, j] = d[i - 1, j - 1]; //no operation
                    else
                        d[i, j] = Math.Min(Math.Min(
                            d[i - 1, j] + 1, //a deletion
                            d[i, j - 1] + 1), //an insertion
                            d[i - 1, j - 1] + 1 //a substitution
                            );
            return d[l1, l2];
        }
    }
}