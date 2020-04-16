namespace In.ProjectEKA.HipService.Discovery.Matcher
{
    using System;

    public class AgeGroupMatcher
    {
        private readonly byte allowedAgeDifference;

        public AgeGroupMatcher(byte allowedAgeDifference)
        {
            this.allowedAgeDifference = allowedAgeDifference;
        }

        public bool IsMatching(byte requestAge, byte systemAge)
        {
            return Math.Abs(requestAge - systemAge) <= allowedAgeDifference;
        }
    }
}