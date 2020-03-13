namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public class HiDataRange
    {
        public string From { get; }
        
        public string To { get; }

        public HiDataRange(string from, string to)
        {
            From = from;
            To = to;
        }
    }
}