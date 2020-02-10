namespace In.ProjectEKA.HipService.DataFlow
{
    public class DataFrequency
    {
        public DataFrequencyUnit Unit { get; }
        public int Value { get; }
        public int Repeats { get; }

        public DataFrequency(DataFrequencyUnit unit, int value, int repeats)
        {
            Unit = unit;
            Value = value;
            Repeats = repeats;
        }
    }
}