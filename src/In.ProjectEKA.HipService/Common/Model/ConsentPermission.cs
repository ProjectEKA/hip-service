namespace In.ProjectEKA.HipService.Common.Model
{
    using System;

    public class ConsentPermission
    {
        public AccessMode AccessMode { get; }
        public AccessPeriod DateRange { get; }
        public DataFrequency Frequency { get; }
        public DateTime DataExpiryAt { get; }

        public ConsentPermission(AccessMode accessMode, AccessPeriod dateRange, DataFrequency frequency,
            DateTime dataExpiryAt)
        {
            AccessMode = accessMode;
            DateRange = dateRange;
            Frequency = frequency;
            DataExpiryAt = dataExpiryAt;
        }
    }
}