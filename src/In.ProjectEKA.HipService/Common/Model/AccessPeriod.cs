namespace In.ProjectEKA.HipService.Common.Model
{
    using System;

    public class AccessPeriod
    {
        public DateTime From { get; }
        public DateTime To { get; }

        public AccessPeriod(DateTime @from, DateTime date)
        {
            From = @from;
            To = date;
        }
    }
}