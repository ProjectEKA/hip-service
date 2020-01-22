using System;

namespace In.ProjectEKA.DefaultHip.Link
{
    public class ReferenceNumberGenerator : IReferenceNumberGenerator
    {
        public string NewGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}