namespace In.ProjectEKA.HipService.Link
{
    using System;

    public class ReferenceNumberGenerator
    {
        public virtual string NewGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}