namespace In.ProjectEKA.HipService.DataFlow
{
    public class DateRange
    {
        public string From { get; }
        public string To { get; }

        public DateRange(string from, string to)
        {
            From = from;
            To = to;
        }
    }
}