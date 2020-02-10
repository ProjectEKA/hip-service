namespace In.ProjectEKA.HipService.DataFlow
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