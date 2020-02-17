namespace In.ProjectEKA.HipService.Common.Model
{
    using System;

    public class AccessPeriod
    {
        public DateTime FromDate { get; }
        public DateTime ToDate { get; }

        public AccessPeriod(DateTime fromDate, DateTime date)
        {
            FromDate = fromDate;
            ToDate = date;
        }
    }
}