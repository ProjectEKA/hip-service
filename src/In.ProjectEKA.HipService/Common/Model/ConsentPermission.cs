namespace In.ProjectEKA.HipService.Common.Model
{
    using System;

    public class ConsentPermission
    {
        public ConsentPermission(AccessMode accessMode,
            AccessPeriod dateRange,
            DataFrequency frequency,
            DateTime dataEraseAt)
        {
            AccessMode = accessMode;
            DateRange = dateRange;
            Frequency = frequency;
            DataEraseAt = dataEraseAt;
        }

        public AccessMode AccessMode { get; }
        public AccessPeriod DateRange { get; }
        public DataFrequency Frequency { get; }
        public DateTime DataEraseAt { get; }
    }
}