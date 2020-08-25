namespace In.ProjectEKA.HipService.Common.Model
{
    using System;

    public class AccessPeriod
    {
        public AccessPeriod(DateTime from, DateTime date)
        {
            From = from;
            To = date;
        }

        public DateTime From { get; }
        public DateTime To { get; }
    }
}