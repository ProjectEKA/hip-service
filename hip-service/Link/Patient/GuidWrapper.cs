using System;

namespace hip_service.Link.Patient
{
    public class GuidWrapper: IGuidWrapper
    {
        public string NewGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}