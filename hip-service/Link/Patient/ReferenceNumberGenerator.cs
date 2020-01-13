using System;

namespace hip_service.Link.Patient
{
    public class ReferenceNumberGenerator: IReferenceNumberGenerator
    {
        public string NewGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}