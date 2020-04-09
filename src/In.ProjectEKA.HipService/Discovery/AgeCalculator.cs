namespace In.ProjectEKA.HipService.Discovery
{
    using System;

    public class AgeCalculator
    {
        public static byte From(ushort yearOfBirth)
        {
            return (byte) (DateTime.Today.Year - yearOfBirth);
        }
    }
}