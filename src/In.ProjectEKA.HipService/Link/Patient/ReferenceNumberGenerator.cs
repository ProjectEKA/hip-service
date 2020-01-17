namespace In.ProjectEKA.HipService.Link.Patient
{
    using System;

    public class ReferenceNumberGenerator : IReferenceNumberGenerator
    {
        public string NewGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}