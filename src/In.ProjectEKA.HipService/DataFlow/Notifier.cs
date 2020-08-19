namespace In.ProjectEKA.HipService.DataFlow
{
    public class Notifier
    {
        public Notifier(Type type, string id)
        {
            Type = type;
            Id = id;
        }

        public Type Type { get; }
        public string Id { get; }
    }
}